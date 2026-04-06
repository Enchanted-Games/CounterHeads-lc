using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CounterHeads.MonoBehaviours;

public class CoilDeathTimer : MonoBehaviour
{
    private float dieAt = -1;
    private SpringManAI deadCoil;

    public void SetDead(SpringManAI coil)
    {
        this.dieAt = Time.fixedTime + 0.5f + (Random.value * 0.25f);
        this.deadCoil = coil;
        this.deadCoil.SetCoilheadOnCooldownServerRpc(true);
        this.deadCoil.SetCoilheadOnCooldownClientRpc(true);
        CounterHeads.Logger.LogMessage($"Coil died. time: {Time.fixedTime}, dieAt: {this.dieAt}");
    }

    public bool MarkedForDeath()
    {
        return this.deadCoil != null && this.dieAt >= 0;
    }

    public void Update()
    {
        if(this.dieAt < 0) return;
        if(this.dieAt > Time.fixedTime) return;
        this.dieAt = -1;
        
        var pos = this.deadCoil.serverPosition;
        this.deadCoil.KillEnemyOnOwnerClient(true);
        Landmine.SpawnExplosion(pos, spawnExplosionEffect: true, killRange: 1f, damageRange: 4f, nonLethalDamage: 35, physicsForce: 25f);
    }
}