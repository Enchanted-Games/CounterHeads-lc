using CounterHeads.Config.Value;
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
        SpringManAI? coil = __instance as SpringManAI;
        if(coil == null) return;
        
        __instance.gameObject.AddComponent<CoilDeathTimer>();
        var deathTimer = __instance.gameObject.GetComponent<CoilDeathTimer>();
        deathTimer.SetCoil(coil);
    }
    
    [HarmonyPatch(nameof(EnemyAI.Start))]
    [HarmonyPostfix]
    static void Start_Postfix(EnemyAI __instance)
    {
        SpringManAI? coil = __instance as SpringManAI;
        if(coil == null) return;

        int newHealth = CounterHeads.SyncedConfig.Get().CoilHealth;
        if (coil.IsOwner && newHealth != 3)
        {
            coil.enemyHP = newHealth;
        }
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
        
        if (coil.isEnemyDead || deathTimer.MarkedForDeath() || coil.enemyHP < 0)
            return;

        WeaponMap.Weapon? weaponData = CounterHeads.SyncedConfig.Get().CoilWeapons
            .TryGetWeaponData(itemHitWith.itemProperties.itemName);
        if(weaponData == null)
            return;

        int damage = weaponData.Value.GetDamage() == 0 ? force : weaponData.Value.GetDamage();
        coil.enemyHP -= damage;
        
        CounterHeads.Instance.LogInfoIfExtendedLogging($"new health: {coil.enemyHP}, damagedBy: {damage}");
        
        if (coil.enemyHP > 0 || !coil.IsOwner)
            return;
        
        if (deathTimer != null)
        {
            CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilPatch::HitPostfix on server: {__instance.IsServer}");
            deathTimer.SetDead();
        }
    }
}
