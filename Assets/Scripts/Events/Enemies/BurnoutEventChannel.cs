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

[CreateAssetMenu(fileName = "BurnoutEventChannel", menuName = "EventChannels/BurnoutEventChannel")]
public class BurnoutEventChannel : ScriptableObject
{
    public BurnoutReceiverActivationEventChannel OnActivationChanged { get; private set; } = new BurnoutReceiverActivationEventChannel();
    public BurnoutDeathEventChannel OnDeath { get; private set; } = new BurnoutDeathEventChannel();
}
