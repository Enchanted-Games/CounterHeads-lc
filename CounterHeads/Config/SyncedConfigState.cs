using CounterHeads.Config.Value;

namespace CounterHeads.Config;

public class SyncedConfigState(
    int coilHealth,
    WeaponMap coilWeapons,
    bool coilsExplode,
    bool coilStunOnDeath,
    int explosionDamage,
    double minTimeUntilExplosion,
    double maxTimeUntilExplosion
)
{
    public readonly int CoilHealth = coilHealth;
    public readonly WeaponMap CoilWeapons = coilWeapons;
    public readonly bool CoilsExplode = coilsExplode;
    public readonly bool CoilStunOnDeath = coilStunOnDeath;
    public readonly int ExplosionDamage = explosionDamage;
    public readonly double MinTimeUntilExplosion = minTimeUntilExplosion;
    public readonly double MaxTimeUntilExplosion = maxTimeUntilExplosion;
    
    public static SyncedConfigState CreateFromCurrentLocal()
    {
        return new SyncedConfigState(
            CounterHeads.LocalConfig.CoilHealth.Value,
            WeaponMap.ParseWeaponConfig(CounterHeads.LocalConfig.CoilWeapons.Value),
            CounterHeads.LocalConfig.CoilsExplode.Value,
            CounterHeads.LocalConfig.CoilStunOnDeath.Value,
            CounterHeads.LocalConfig.ExplosionDamage.Value,
            CounterHeads.LocalConfig.MinTimeUntilExplosion.Value,
            CounterHeads.LocalConfig.MaxTimeUntilExplosion.Value
        );
    }
}