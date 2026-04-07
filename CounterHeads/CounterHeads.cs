using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OPJosMod.ReviveCompany;
using CounterHeads.Config;

namespace CounterHeads;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class CounterHeads : BaseUnityPlugin
{
    public static CounterHeads Instance { get; private set; } = null!;
    public static ConfigManager ConfigManager { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        ConfigManager = new ConfigManager(Config);

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }

    public void LogInfoIfExtendedLogging(string message)
    {
        if(!ConfigManager.ExtendedLogging.Value)
            return;
        Logger.LogInfo(message);
    }
}
