using System;
using CounterHeads.Networking;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CounterHeads.MonoBehaviours;

public class CoilDeathTimer : NetworkBehaviour
{
    private const float StartingPitch = 0.8f;
    private const float EndingPitch = 1.2f;

    private double _warningAudioStopAt = -1f;
    private double _dieAt = -1;
    private SpringManAI _coil = null!;
    private AudioSource _deathWarningSource = null!;
    private AudioClip? _deathWarningAudio;

    public void Awake()
    {
        _deathWarningAudio = CounterHeads.AssetBundle?.LoadAsset<AudioClip>("CoilWarning");
    }

    public void SetCoil(SpringManAI coil)
    {
        _coil = coil;
        _deathWarningSource = _coil.creatureSFX;
    }

    public void SetDead()
    {
        if(!IsServer)
        {
            CounterHeads.Instance.LogInfoIfExtendedLogging("CoilDeathTimer::SetDead called on a non-server");
            return;
        }

        if (CounterHeads.SyncedConfig.Get().CoilsExplode)
        {
            double minAudioDur = CounterHeads.SyncedConfig.Get().MinTimeUntilExplosion;
            double warningAudioDuration = Random.value * (CounterHeads.SyncedConfig.Get().MaxTimeUntilExplosion - minAudioDur) + minAudioDur;
            SendAudioWarningDurationToEveryone(warningAudioDuration);
            
            _dieAt = Time.fixedTimeAsDouble + warningAudioDuration;
            
            if (CounterHeads.SyncedConfig.Get().CoilStunOnDeath)
            {
                _coil.SetCoilheadOnCooldownClientRpc(true);
            }
            
            CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilDeathTimer::SetDead on server: {IsServer}, dieAt: {_dieAt}, warningAudioDuration: {warningAudioDuration}");
        }
        else
        {
            CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilDeathTimer::SetDead on server: {IsServer}, dieAt: {_dieAt}. Explosion config disabled so we're just killing the coilhead");
            KillCoil();
        }
    }

    public bool MarkedForDeath()
    {
        return _dieAt >= 0;
    }

    public void Update()
    {
        if (_warningAudioStopAt >= 0 && _warningAudioStopAt <= Time.fixedTimeAsDouble)
        {
            StopPlayingAudio();
        }
        
        if(!IsServer) return;
        if(!_coil) return;
        if(!MarkedForDeath() || Time.fixedTimeAsDouble < _dieAt) return;
        
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilDeathTimer::Update on server: {IsServer}. dieAt: {_dieAt}, fixedTime: {Time.fixedTimeAsDouble}, coilExists: {_coil}");
        KillCoil();
        _dieAt = -1;
    }

    public void KillCoil()
    {
        _coil.KillEnemyOnOwnerClient(true);
        
        if (!CounterHeads.SyncedConfig.Get().CoilsExplode)
            return;
        
        const float killRange = 1f;
        const float damageRange = 4f;
        int nonLethalDamage = CounterHeads.SyncedConfig.Get().ExplosionDamage;
        const float physicsForce = 25f;
        
        var pos = _coil.serverPosition;
        SendExplosionEffectToEveryone(pos, killRange: killRange, damageRange: damageRange, nonLethalDamage: nonLethalDamage, physicsForce: physicsForce);
    }
 
    private void SetupAudioLoop(double duration)
    {
        _warningAudioStopAt = Time.fixedTimeAsDouble + duration;
        _deathWarningSource.loop = true;
        _deathWarningSource.volume = 5f;
        _deathWarningSource.pitch = 0.7f;
        if (_deathWarningAudio)
        {
            _deathWarningSource.clip = _deathWarningAudio;
        }
        
        if (_deathWarningAudio != null)
        {
            _deathWarningSource.Play();
        }
    }

    public void StopPlayingAudio()
    {
        if (_deathWarningSource)
        {
            _deathWarningSource.Stop();
        }
    }
    
    
    public override void OnNetworkSpawn()
    {
        NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(Messages.CoilDeathTimer_AudioWarningToEveryoneMessage, ReceiveAudioWarningDuration);
    }
    public override void OnNetworkDespawn()
    {
        NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(Messages.CoilDeathTimer_AudioWarningToEveryoneMessage);
    }
    
    public void SendAudioWarningDurationToEveryone(double duration)
    {
        var buffer = new FastBufferWriter(1024, Allocator.Temp);
        buffer.WriteValue(duration);
        NetworkManager.CustomMessagingManager.SendNamedMessageToAll(Messages.CoilDeathTimer_AudioWarningToEveryoneMessage, buffer);
    }

    public void ReceiveAudioWarningDuration(ulong senderId, FastBufferReader data)
    {
        try
        {
            data.ReadValue(out double duration);

            CounterHeads.Instance.LogInfoIfExtendedLogging($"{nameof(ReceiveAudioWarningDuration)} received: {duration}");
            SetupAudioLoop(duration);
        }
        catch (Exception ex)
        {
            CounterHeads.Logger.LogError($"Exception during networking: {ex}");
        }
    }
    
    

    public static void SendExplosionEffectToEveryone(Vector3 pos, float killRange, float damageRange, int nonLethalDamage, float physicsForce)
    {
        var buffer = new FastBufferWriter(1024, Allocator.Temp);
        buffer.WriteValue(pos);
        buffer.WriteValue(killRange);
        buffer.WriteValue(damageRange);
        buffer.WriteValue(nonLethalDamage);
        buffer.WriteValue(physicsForce);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(Messages.Global_ExplosionEffectToEveryoneMessage, buffer);
        CounterHeads.Instance.LogInfoIfExtendedLogging($"Sent explosion effects {pos} {killRange} {damageRange} {nonLethalDamage} {physicsForce}");
    }

    public static void ReceiveExplosionEffect(ulong senderId, FastBufferReader data)
    {
        try
        {
            data.ReadValue(out Vector3 pos);
            data.ReadValue(out float killRange);
            data.ReadValue(out float damageRange);
            data.ReadValue(out int nonLethalDamage);
            data.ReadValue(out float physicsForce);
            CounterHeads.Instance.LogInfoIfExtendedLogging($"{nameof(ReceiveExplosionEffect)} received: {pos} {killRange} {damageRange} {nonLethalDamage} {physicsForce}");
            Landmine.SpawnExplosion(pos, spawnExplosionEffect: true, killRange: killRange, damageRange: damageRange, nonLethalDamage: nonLethalDamage, physicsForce: physicsForce);
        }
        catch (Exception ex)
        {
            CounterHeads.Logger.LogError($"Exception during networking: {ex}");
        }
    }
}