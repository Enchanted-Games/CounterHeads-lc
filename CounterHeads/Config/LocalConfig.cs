using BepInEx.Configuration;

namespace CounterHeads.Config;

public class LocalConfig(ConfigFile config)
{
    private const string DamageSection = "Damage";
    private const string BehaviourSection = "Behaviour";
    private const string DebuggingSection = "Debugging";
    
    // - Damage
    public readonly ConfigEntry<int> CoilHealth = config.Bind(
        DamageSection,
        "Coilhead health",
        8,
        new ConfigDescription(
            "How much health coilheads spawn with. 3 is the vanilla default, so setting this value to 3 will not modify coilhead health. This is useful if, for example, you want to use the coilhead health from another mod instead of overriding it",
            new AcceptableValueRange<int>(1, 500)
        )
    );
    
    public readonly ConfigEntry<string> CoilWeapons = config.Bind(
        DamageSection,
        "Coilhead weapons",
        "{\'kitchen knife\':3},{\'shotgun\':2}",
        "Configure which weapons can be used against coilheads and how much damage they should deal.\nEntries are surrounded by curly brackets {} and seperated by a comma. Each entry can contain either a weapon name like so: `{'shotgun'}` (in this case the default damage amount for that item will be used), or a weapon name and damage amount seperated by a colon, like so: `{'shotgun':2}`. Weapon names should be as they appear in the top right of the screen while holding them, if the weapon name contains a `:` or `'` character, you can escape them like so: `\\:` or `\\'`\n\nAn example config to make shovels deal 2 damage and knives deal 1 could look like this: `{'shovel':2},{'kitchen knife':1}`"
    );
    
    // - Behaviour
    public readonly ConfigEntry<bool> CoilsExplode = config.Bind(
        BehaviourSection,
        "Coilheads explode",
        true,
        "Whether or not coilheads explode on death. If enabled, coilheads will start playing a warning sound which will get higher pitch until eventually exploding. If disabled, coilheads will vanish on death and no warning sound will play"
    );
    
    public readonly ConfigEntry<bool> CoilStunOnDeath = config.Bind(
        BehaviourSection,
        "Stun coilheads on death",
        false,
        "Whether or not coilheads should be stunned when dealing enough damage to them. If disabled, coilheads will still be able to chase you until they explode!\nOnly takes effect when 'Coilheads explode' is enabled"
    );
    
    public readonly ConfigEntry<int> ExplosionDamage = config.Bind(
        BehaviourSection,
        "Explosion damage",
        45,
        new ConfigDescription(
            "Maximum amount of damage coilhead explosions can inflict\nOnly takes effect when 'Coilheads explode' is enabled",
            new AcceptableValueRange<int>(0, 200)
        )
    );
    
    public readonly ConfigEntry<double> MinTimeUntilExplosion = config.Bind(
        BehaviourSection,
        "Min time until explosion",
        0.6d,
        new ConfigDescription(
            "Minimum amount of time in seconds between dealing enough damage to a coilhead and it exploding\nOnly takes effect when 'Coilheads explode' is enabled",
            new AcceptableValueRange<double>(0d, 20d)
        )
    );
    
    public readonly ConfigEntry<double> MaxTimeUntilExplosion = config.Bind(
        BehaviourSection,
        "Max time until explosion",
        1d,
        new ConfigDescription(
            "Maximum amount of time in seconds between dealing enough damage to a coilhead and it exploding\nOnly takes effect when 'Coilheads explode' is enabled",
            new AcceptableValueRange<double>(0d, 20d)
        )
    );
    
    
    // - Debugging
    public readonly ConfigEntry<bool> ExtendedLogging = config.Bind(
        DebuggingSection,
        "Extended logging",
        false,
        "Enable extra logging for debugging"
    );
}