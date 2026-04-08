using System.IO;
using BepInEx;
using BepInEx.Logging;
using CounterHeads.Assets;
using HarmonyLib;
using CounterHeads.Config;
using UnityEngine;

namespace CounterHeads;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class CounterHeads : BaseUnityPlugin
{
    public static CounterHeads Instance { get; private set; } = null!;
    public static ConfigManager ConfigManager { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    public static AssetBundle? AssetBundle;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        ConfigManager = new ConfigManager(Config);
        Patch();
        LoadAssets();

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

    internal void LoadAssets()
    {
        var pluginPath = Path.GetDirectoryName(Info.Location);
        if (pluginPath == null)
        {
            AssetBundle = null;
            Logger.LogError("Could not find plugin location");
            return;
        }
        var bundlePath = Path.Combine(pluginPath, "counter-heads-assets");
        AssetBundleUtil.LoadAssetBundle(bundlePath);
        AssetBundle = AssetBundleUtil.GetBundle();
    }

    public void LogInfoIfExtendedLogging(string message)
    {
        if(!ConfigManager.ExtendedLogging.Value)
            return;
        Logger.LogInfo(message);
    }
}
