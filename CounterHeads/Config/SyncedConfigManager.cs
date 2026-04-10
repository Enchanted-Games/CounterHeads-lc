using System;
using Unity.Netcode;

namespace CounterHeads.Config;

public class SyncedConfigManager
{
    private SyncedConfigState _currentState = SyncedConfigState.CreateFromCurrentLocal();
    public bool ServerHasCounterHeads { get; private set; } = false;

    public SyncedConfigState Get()
    {
        return _currentState;
    }

    public void SetToServersConfig(SyncedConfigState state)
    {
        CounterHeads.Instance.LogInfoIfExtendedLogging("Recieved config state from server.");
        CounterHeads.SyncedConfig.Get().LogCurrentState();
        _currentState = state;
        CounterHeads.Instance.LogInfoIfExtendedLogging("Setting to servers config");
        CounterHeads.SyncedConfig.Get().LogCurrentState();
    }

    public void ResetToLocal()
    {
        CounterHeads.Instance.LogInfoIfExtendedLogging("Resetting config back to local values");
        _currentState = SyncedConfigState.CreateFromCurrentLocal();
        ServerHasCounterHeads = false;
        CounterHeads.SyncedConfig.Get().LogCurrentState();
    }

    public void ReceiveConfigSync(ulong id, FastBufferReader data)
    {
        ServerHasCounterHeads = true;
        if(NetworkManager.Singleton.IsServer)
            return;
        try
        {
            data.ReadNetworkSerializable(out SyncedConfigState configState);
            CounterHeads.Instance.LogInfoIfExtendedLogging($"{nameof(ReceiveConfigSync)} received: {configState}");
            SetToServersConfig(configState);
        }
        catch (Exception ex)
        {
            CounterHeads.Logger.LogError($"Exception during networking: {ex}");
        }
    }

    public void DisconnectFromServer()
    {
        ResetToLocal();
    }
}