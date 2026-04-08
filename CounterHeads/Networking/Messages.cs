using CounterHeads.MonoBehaviours;
using Unity.Netcode;

namespace CounterHeads.Networking;

public static class Messages
{
    public const string Global_ExplosionEffectToEveryoneMessage = "CounterHeads_Global_ExplosionEffectToEveryone";
    
    public const string CoilDeathTimer_AudioWarningToEveryoneMessage = "CounterHeads_CoilDeathTimer_AudioWarningDurationToEveryone";

    public static void RegisterGlobalMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(Global_ExplosionEffectToEveryoneMessage, CoilDeathTimer.ReceiveExplosionEffect);
    }
    
    public static void UnregisterGlobalMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(Global_ExplosionEffectToEveryoneMessage);
    }
}