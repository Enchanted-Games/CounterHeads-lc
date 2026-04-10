namespace CounterHeads.Config;

public class SyncedConfigManager
{
    private SyncedConfigState _currentState = SyncedConfigState.CreateFromCurrentLocal();

    public SyncedConfigState Get()
    {
        return _currentState;
    }
}