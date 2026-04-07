using BepInEx.Configuration;

namespace CounterHeads.Config;

public class ConfigManager
{
    public ConfigEntry<bool> ExtendedLogging;
    
    public ConfigManager(ConfigFile config)
    {
        ExtendedLogging = config.Bind(
            "Debugging",
            "Extended Logging",
            false,
            "Enable extra logging for debugging"
        );
    }
}