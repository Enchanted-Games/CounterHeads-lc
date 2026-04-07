using CounterHeads.Networking;
using HarmonyLib;
using Unity.Netcode;

namespace CounterHeads.Patches;

[HarmonyPatch]
internal class NetworkManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.Initialize))]
    private static void AfterInitialize()
    {
        CounterHeads.Logger.LogInfo("Registering Messages");
        CoilDeathTimerMessages.RegisterMessages();
    }

    [HarmonyPatch(typeof(GameNetworkManager), "SetInstanceValuesBackToDefault")]
    [HarmonyPostfix]
    public static void SetInstanceValuesBackToDefault()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.CustomMessagingManager == null)
            return;

        CounterHeads.Logger.LogInfo("Unregistering Messages");
        CoilDeathTimerMessages.UnregisterMessages();
    }
}