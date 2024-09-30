using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PortalStartedEvent
{
    public float PortalLength { get; private set; }

    public PortalStartedEvent(float portalLength)
    {
        PortalLength = portalLength;
    }
}

public class PortalStartedEventChannel : AbstractEventChannel<PortalStartedEvent>
{

}

public struct PortalOpenedEvent
{
    public Vector2 Position { get; private set; }
    public Vector2 Orientation { get; private set; }
    public Vector3Int[] AffectedTiles { get; private set; }

    public PortalOpenedEvent(Vector2 position, Vector2 orientation, Vector3Int[] affectedTiles)
    {
        Position = position;
        Orientation = orientation;
        AffectedTiles = affectedTiles;
    }
}

public class PortalOpenedEventChannel : AbstractEventChannel<PortalOpenedEvent>
{

}

public struct PortalClearedEvent
{

}

public class PortalClearedEventChannel : AbstractEventChannel<PortalClearedEvent>
{

}

[CreateAssetMenu(fileName = "PortalEventChannel", menuName = "EventChannels/PortalEventChannel")]
public class PortalEventChannel : ScriptableObject 
{
    public PortalStartedEventChannel OnPortalStarted { get; private set; } = new PortalStartedEventChannel();
    public PortalOpenedEventChannel OnPortalOpened { get; private set; } = new PortalOpenedEventChannel();
    public PortalClearedEventChannel OnPortalCleared { get; private set; } = new PortalClearedEventChannel();
}
