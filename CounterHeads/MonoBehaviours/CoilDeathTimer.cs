using System;
using CounterHeads.Networking;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CounterHeads.MonoBehaviours;

public class CoilDeathTimer : NetworkBehaviour
{
    private float _dieAt = -1;
    private SpringManAI? _coil;

    public void SetDead(SpringManAI coil)
    {
        if(!IsServer) return;
        _coil = coil;
        _dieAt = Time.fixedTime + 0.8f + (Random.value * 0.25f);
        coil.SetCoilheadOnCooldownServerRpc(true);
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilDeathTimer::SetDead on server: {IsServer}");
    }

    public bool MarkedForDeath()
    {
        return _dieAt >= 0;
    }

    public void Update()
    {
        if(!IsServer) return;
        if(!MarkedForDeath()) return;
        if(_dieAt > Time.fixedTime) return;
        if(_coil == null) return;
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilDeathTimer::Update on server: {IsServer}");

        _dieAt = -1;
        var pos = _coil.serverPosition;
        _coil.KillEnemyOnOwnerClient(true);

        const float killRange = 1f;
        const float damageRange = 4f;
        const int nonLethalDamage = 35;
        const float physicsForce = 25f;
        
        SendExplosionEffectToClients(pos, killRange: killRange, damageRange: damageRange, nonLethalDamage: nonLethalDamage, physicsForce: physicsForce);
    }

    public static void SendExplosionEffectToClients(Vector3 pos, float killRange, float damageRange, int nonLethalDamage, float physicsForce)
    {
        var buffer = new FastBufferWriter(1024, Allocator.Temp);
        buffer.WriteValue(pos);
        buffer.WriteValue(killRange);
        buffer.WriteValue(damageRange);
        buffer.WriteValue(nonLethalDamage);
        buffer.WriteValue(physicsForce);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(CoilDeathTimerMessages.ExplosionEffectToClientsMessage, buffer);
    }

    public static void ReceiveExplosionEffectClient(ulong senderId, FastBufferReader data)
    {
        if (senderId != NetworkManager.ServerClientId)
            return;
        
        try
        {
            data.ReadValue(out Vector3 pos);
            data.ReadValue(out float killRange);
            data.ReadValue(out float damageRange);
            data.ReadValue(out int nonLethalDamage);
            data.ReadValue(out float physicsForce);

            CounterHeads.Instance.LogInfoIfExtendedLogging($"SendExplosionEffectToClients received: {pos} {killRange} {damageRange} {nonLethalDamage} {physicsForce}");
            
            Landmine.SpawnExplosion(pos, spawnExplosionEffect: true, killRange: killRange, damageRange: damageRange, nonLethalDamage: nonLethalDamage, physicsForce: physicsForce);
        }
        catch (Exception ex)
        {
            CounterHeads.Logger.LogError($"Exception during networking: {ex}");
        }
    }
}