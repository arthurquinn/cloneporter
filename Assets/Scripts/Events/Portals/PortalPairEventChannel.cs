using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PortalPairStartedEvent
{
    public float PortalLength { get; private set; }

    public PortalPairStartedEvent(float portalLength)
    {
        PortalLength = portalLength;
    }
}

public class PortalPairStartedEventChannel : AbstractEventChannel<PortalPairStartedEvent>
{

}

public struct PortalPairClearedEvent
{

}

public class PortalPairClearedEventChannel : AbstractEventChannel<PortalPairClearedEvent>
{

}

public enum PortalActiveState
{
    Active,
    Inactive,
}

public struct PortalActiveStateEvent
{
    public PortalActiveState State { get; private set; }

    public PortalActiveStateEvent(PortalActiveState state)
    {
        State = state;
    }
}

public class PortalActiveStateEventChannel : AbstractEventChannel<PortalActiveStateEvent>
{

}


[CreateAssetMenu(fileName = "PortalPairEventChannel", menuName = "EventChannels/PortalPairEventChannel")]
public class PortalPairEventChannel : ScriptableObject
{
    public PortalPairStartedEventChannel OnPortalPairStarted { get; private set; } = new PortalPairStartedEventChannel();
    public PortalPairClearedEventChannel OnPortalPairCleared { get; private set; } = new PortalPairClearedEventChannel();
    public PortalActiveStateEventChannel OnPortalActiveState { get; private set; } = new PortalActiveStateEventChannel();
}
