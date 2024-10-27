using UnityEngine;

public enum BurnoutReceiverActivationType
{
    Activated,
    Deactivated
}

public struct BurnoutReceiverActivationEvent
{
    public string Name;
    public BurnoutReceiverActivationType Type;

    public BurnoutReceiverActivationEvent(string name, BurnoutReceiverActivationType type)
    {
        Name = name;
        Type = type;
    }
}

public class BurnoutReceiverActivationEventChannel : AbstractEventChannel<BurnoutReceiverActivationEvent>
{

}

public struct BurnoutDeathEvent
{
    public string Name;

    public BurnoutDeathEvent(string name)
    {
        Name = name;
    }
}

public class BurnoutDeathEventChannel : AbstractEventChannel<BurnoutDeathEvent>
{

}

public enum BurnoutLaserPortalEventType
{
    Enter,
    Exit
}

public struct BurnoutLaserPortalEvent
{
    public BurnoutLaserPortalEventType Type;

    public BurnoutLaserPortalEvent(BurnoutLaserPortalEventType type)
    {
        Type = type;
    }
}

public class BurnoutLaserPortalEventChannel : AbstractEventChannel<BurnoutLaserPortalEvent>
{

}

[CreateAssetMenu(fileName = "BurnoutEventChannel", menuName = "EventChannels/BurnoutEventChannel")]
public class BurnoutEventChannel : ScriptableObject
{
    public BurnoutReceiverActivationEventChannel OnActivationChanged { get; private set; } = new BurnoutReceiverActivationEventChannel();
    public BurnoutDeathEventChannel OnDeath { get; private set; } = new BurnoutDeathEventChannel();
    public BurnoutLaserPortalEventChannel OnPortalEvent { get; private set; } = new BurnoutLaserPortalEventChannel();
}
