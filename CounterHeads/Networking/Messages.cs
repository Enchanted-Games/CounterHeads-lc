using CounterHeads.Config;
using CounterHeads.MonoBehaviours;
using Unity.Collections;
using Unity.Netcode;

namespace CounterHeads.Networking;

public static class Messages
{
    public const string Global_ConfigSync = "CounterHeads_Global_ConfigSync";
    public const string Global_ExplosionEffectToEveryoneMessage = "CounterHeads_Global_ExplosionEffectToEveryone";
    
    public const string CoilDeathTimer_AudioWarningToEveryoneMessage = "CounterHeads_CoilDeathTimer_AudioWarningDurationToEveryone";

    public static void RegisterGlobalMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(Global_ConfigSync, CounterHeads.SyncedConfig.ReceiveConfigSync);
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(Global_ExplosionEffectToEveryoneMessage, CoilDeathTimer.ReceiveExplosionEffect);
        
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }
    
    public static void UnregisterGlobalMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(Global_ExplosionEffectToEveryoneMessage);
        
        NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnected;
    }

    private static void ClientConnected(ulong clientId)
    {
        CounterHeads.Instance.LogInfoIfExtendedLogging($"Messages::ClientConnected on serer {NetworkManager.Singleton.IsServer}, clientId: {clientId}");
        if(!NetworkManager.Singleton.IsServer)
            return;
        
        CounterHeads.Instance.LogInfoIfExtendedLogging($"Sending config to client {clientId}");
        var buffer = new FastBufferWriter(1024, Allocator.Temp);
        buffer.WriteValue(CounterHeads.SyncedConfig.Get());
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(Global_ConfigSync, clientId, buffer);
    }

    private static void ClientDisconnected(ulong clientId)
    {
        CounterHeads.Instance.LogInfoIfExtendedLogging($"Messages::ClientDisconnected on serer {NetworkManager.Singleton.IsServer}, clientId: {clientId}");
        if(NetworkManager.Singleton.IsServer)
            return;
        CounterHeads.SyncedConfig.DisconnectFromServer();
    }
}