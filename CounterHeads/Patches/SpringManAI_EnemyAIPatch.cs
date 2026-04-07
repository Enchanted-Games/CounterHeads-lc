using CounterHeads.MonoBehaviours;
using GameNetcodeStuff;
using HarmonyLib;

namespace CounterHeads.Patches;

[HarmonyPatch(typeof(EnemyAI))]
public class SpringManAI_EnemyAIPatch
{
    [HarmonyPatch(nameof(EnemyAI.Awake))]
    [HarmonyPostfix]
    static void Awake_Postfix(EnemyAI __instance)
    {
        SpringManAI? spring = __instance as SpringManAI;
        if(spring == null) return;
        
        __instance.gameObject.AddComponent<CoilDeathTimer>();
    }
    
    [HarmonyPatch(nameof(EnemyAI.HitEnemy))]
    [HarmonyPostfix]
    static void HitEnemy_Postfix(EnemyAI __instance, int force, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID)
    {
        SpringManAI? coil = __instance as SpringManAI;
        if(coil == null) return;
        if(playerWhoHit == null) return;

        GrabbableObject itemHitWith = playerWhoHit.currentlyHeldObjectServer;
        CounterHeads.Instance.LogInfoIfExtendedLogging($"SpringManAI hit. force: {force}, player: {playerWhoHit.playerUsername}, health: {coil.enemyHP}, isDead: {coil.isEnemyDead}, isOwner: {coil.IsOwner}, playerItem: {itemHitWith.itemProperties.itemName}, playerWhoHitIsClient: {playerWhoHit.IsClient}, coilIsClient: {coil.IsClient}");
        
        CoilDeathTimer deathTimer = coil.GetComponent<CoilDeathTimer>();
        
        if (coil.isEnemyDead || deathTimer.MarkedForDeath())
            return;
        if(!itemHitWith.itemProperties.itemName.ToLower().Equals("kitchen knife"))
            return;
        
        coil.enemyHP -= force * (coil.inCooldownAnimation ? 2 : 1);
        
        CounterHeads.Instance.LogInfoIfExtendedLogging($"new health: {coil.enemyHP}");
        
        if (coil.enemyHP > 0)
            return;
        
        if (deathTimer != null)
        {
            CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilPatch::HitPostfix on server: {__instance.IsServer}");
            deathTimer.SetDead(coil);
        }
    }
}
