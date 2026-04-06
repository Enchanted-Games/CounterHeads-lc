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
        CounterHeads.Logger.LogDebug($"SpringManAI hit. force: {force}, player: {playerWhoHit.playerUsername}, health: {coil.enemyHP}, isDead: {coil.isEnemyDead}, isOwner: {coil.IsOwner}, playerItem: {itemHitWith.itemProperties.itemName}");
        
        CoilDeathTimer deathTimer = coil.GetComponent<CoilDeathTimer>();
        
        if (coil.isEnemyDead || deathTimer.MarkedForDeath())
            return;
        if(!itemHitWith.itemProperties.itemName.ToLower().Equals("kitchen knife"))
            return;
        
        coil.enemyHP -= force * (coil.inCooldownAnimation ? 2 : 1);
        
        CounterHeads.Logger.LogDebug($"new health: {coil.enemyHP}");
        
        if (coil.enemyHP > 0 || !coil.IsOwner)
            return;
        
        if (deathTimer != null)
        {
            deathTimer.SetDead(coil);
        }
    }
    
    // [HarmonyPatch(nameof(SprayPaintItem.LateUpdate))]
    // [HarmonyTranspiler]
    // static IEnumerable<CodeInstruction> LateUpdate_CadaverKillRate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    // {
    //     // IL_01b7: ldarg.0      // this
    //     // IL_01b8: ldfld        class CadaverGrowthAI SprayPaintItem::cadaverGrowthAI
    //     // IL_01bd: ldfld        class BatchAllMeshChildren[] CadaverGrowthAI::plantBatchers
    //     // IL_01c2: ldarg.0      // this
    //     // IL_01c3: ldflda       valuetype [netstandard]System.ValueTuple`3<int32, int32, int32> SprayPaintItem::killingCadaverPlant
    //     // IL_01c8: ldfld        !2/*int32*/ valuetype [netstandard]System.ValueTuple`3<int32, int32, int32>::Item3
    //     // IL_01cd: ldelem.ref
    //     // IL_01ce: ldfld        class [netstandard]System.Collections.Generic.List`1<class [netstandard]System.Collections.Generic.List`1<valuetype [UnityEngine.CoreModule]UnityEngine.Matrix4x4>> BatchAllMeshChildren::Batches
    //     // IL_01d3: ldarg.0      // this
    //     // IL_01d4: ldflda       valuetype [netstandard]System.ValueTuple`3<int32, int32, int32> SprayPaintItem::killingCadaverPlant
    //     // IL_01d9: ldfld        !0/*int32*/ valuetype [netstandard]System.ValueTuple`3<int32, int32, int32>::Item1
    //     // IL_01de: callvirt     instance !0/*class [netstandard]System.Collections.Generic.List`1<valuetype [UnityEngine.CoreModule]UnityEngine.Matrix4x4>*/ class [netstandard]System.Collections.Generic.List`1<class [netstandard]System.Collections.Generic.List`1<valuetype [UnityEngine.CoreModule]UnityEngine.Matrix4x4>>::get_Item(int32)
    //     // IL_01e3: ldarg.0      // this
    //     // IL_01e4: ldflda       valuetype [netstandard]System.ValueTuple`3<int32, int32, int32> SprayPaintItem::killingCadaverPlant
    //     // IL_01e9: ldfld        !1/*int32*/ valuetype [netstandard]System.ValueTuple`3<int32, int32, int32>::Item2
    //     // IL_01ee: ldloc.0      // matrix4x4_1
    //     // IL_01ef: ldc.r4       2.2
    //     
    //     PatchHelper.LogPatchName(nameof(SpringManAI_EnemyAIPatch), nameof(LateUpdate_CadaverKillRate_Transpiler));
    //     
    //     var codeMatcher = new CodeMatcher(instructions, generator);
    //
    //     codeMatcher.MatchForward(
    //         true,
    //         new CodeMatch(OpCodes.Ldarg_0),
    //         new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SprayPaintItem), nameof(SprayPaintItem.cadaverGrowthAI))),
    //         new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.plantBatchers))),
    //         new CodeMatch(OpCodes.Ldarg_0),
    //         new CodeMatch(i => i.opcode == OpCodes.Ldflda && ((FieldInfo)i.operand).Name == nameof(SprayPaintItem.killingCadaverPlant)),
    //         new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Item3"),
    //         new CodeMatch(OpCodes.Ldelem_Ref),
    //         new CodeMatch(OpCodes.Ldfld),
    //         new CodeMatch(OpCodes.Ldarg_0),
    //         new CodeMatch(i => i.opcode == OpCodes.Ldflda && ((FieldInfo)i.operand).Name == nameof(SprayPaintItem.killingCadaverPlant)),
    //         new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Item1"),
    //         new CodeMatch(OpCodes.Callvirt),
    //         new CodeMatch(OpCodes.Ldarg_0),
    //         new CodeMatch(i => i.opcode == OpCodes.Ldflda && ((FieldInfo)i.operand).Name == nameof(SprayPaintItem.killingCadaverPlant)),
    //         new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Item2"),
    //         new CodeMatch(OpCodes.Ldloc_0),
    //         new CodeMatch(OpCodes.Ldc_R4, 2.2f)
    //     );
    //
    //     PatchHelper.PatchAndLogSurroundingIfValid(
    //         codeMatcher,
    //         matcher =>
    //         {
    //             matcher.Set(OpCodes.Call, AccessTools.DeclaredMethod(typeof(WeedKillerModifiers), nameof(WeedKillerModifiers.GetCadaverKillRate)));
    //         },
    //         "Modify cadaver shrink rate"
    //     );
    //     
    //     return codeMatcher.InstructionEnumeration();
    // }
}
