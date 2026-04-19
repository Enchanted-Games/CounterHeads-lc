using CounterHeads.Config.Value;
using Unity.Netcode;

namespace CounterHeads.Config;

public class SyncedConfigState : INetworkSerializable
{
    public int ServerConfigVersion;
    public int CoilHealth;
    public WeaponMap CoilWeapons;
    public bool CoilsExplode;
    public bool CoilStunOnDeath;
    public int ExplosionDamage;
    public double MinTimeUntilExplosion;
    public double MaxTimeUntilExplosion;

    public SyncedConfigState(
        int serverConfigVersion,
        int coilHealth,
        WeaponMap coilWeapons,
        bool coilsExplode,
        bool coilStunOnDeath,
        int explosionDamage,
        double minTimeUntilExplosion,
        double maxTimeUntilExplosion
    )
    {
        ServerConfigVersion = serverConfigVersion;
        CoilHealth = coilHealth;
        CoilWeapons = coilWeapons;
        CoilsExplode = coilsExplode;
        CoilStunOnDeath = coilStunOnDeath;
        ExplosionDamage = explosionDamage;
        MinTimeUntilExplosion = minTimeUntilExplosion;
        MaxTimeUntilExplosion = maxTimeUntilExplosion;
    }
    
    public SyncedConfigState()
    {
        // for network serializable
    }
    
    public static SyncedConfigState CreateFromCurrentLocal()
    {
        return new SyncedConfigState(
            LocalConfig.ModConfigVersion,
            CounterHeads.LocalConfig.CoilHealth.Value,
            WeaponMap.ParseWeaponConfig(CounterHeads.LocalConfig.CoilWeapons.Value),
            CounterHeads.LocalConfig.CoilsExplode.Value,
            CounterHeads.LocalConfig.CoilStunOnDeath.Value,
            CounterHeads.LocalConfig.ExplosionDamage.Value,
            CounterHeads.LocalConfig.MinTimeUntilExplosion.Value,
            CounterHeads.LocalConfig.MaxTimeUntilExplosion.Value
        );
    }

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref ServerConfigVersion);
        serializer.SerializeValue(ref CoilHealth);
        serializer.SerializeNetworkSerializable(ref CoilWeapons);
        serializer.SerializeValue(ref CoilsExplode);
        serializer.SerializeValue(ref CoilStunOnDeath);
        serializer.SerializeValue(ref ExplosionDamage);
        serializer.SerializeValue(ref MinTimeUntilExplosion);
        serializer.SerializeValue(ref MaxTimeUntilExplosion);
    }

    public void LogCurrentState()
    {
        CounterHeads.Instance.LogInfoIfExtendedLogging($"My current config state is:");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"ServerConfigVersion: {ServerConfigVersion}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilHealth: {CoilHealth}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilWeapons: {CoilWeapons}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilsExplode: {CoilsExplode}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"CoilStunOnDeath: {CoilStunOnDeath}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"ExplosionDamage: {ExplosionDamage}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"MinTimeUntilExplosion: {MinTimeUntilExplosion}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"MaxTimeUntilExplosion: {MaxTimeUntilExplosion}");
        CounterHeads.Instance.LogInfoIfExtendedLogging($"------");
    }
}