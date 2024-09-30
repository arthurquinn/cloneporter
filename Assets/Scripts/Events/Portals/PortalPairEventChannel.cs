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


[CreateAssetMenu(fileName = "PortalPairEventChannel", menuName = "EventChannels/PortalPairEventChannel")]
public class PortalPairEventChannel : ScriptableObject
{
    public PortalPairStartedEventChannel OnPortalPairStarted { get; private set; } = new PortalPairStartedEventChannel();
    public PortalPairClearedEventChannel OnPortalPairCleared { get; private set; } = new PortalPairClearedEventChannel();
}
