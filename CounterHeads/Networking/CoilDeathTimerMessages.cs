using CounterHeads.MonoBehaviours;
using Unity.Netcode;

namespace CounterHeads.Networking;

public class CoilDeathTimerMessages
{
    public const string ExplosionEffectToClientsMessage = "CounterHeads_ExplosionEffectToClients";
    
    internal static void RegisterMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(ExplosionEffectToClientsMessage, CoilDeathTimer.ReceiveExplosionEffectClient);
    }

    internal static void UnregisterMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(ExplosionEffectToClientsMessage);
    }
}