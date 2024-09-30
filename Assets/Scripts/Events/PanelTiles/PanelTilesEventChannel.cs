using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PanelPlacePortalEvent
{
    public PortalColor Color { get; private set; }
    public Vector2 Position {  get; private set; }
    public Vector2 Orientation { get; private set; }
    public Vector3Int[] AffectedTiles { get; private set; }

    public PanelPlacePortalEvent(PortalColor color, Vector2 position, Vector2 orientation, Vector3Int[] affectedTiles)
    {
        Color = color;
        Position = position;
        Orientation = orientation;
        AffectedTiles = affectedTiles;
    }
}

public class PanelPlacePortalEventChannel : AbstractEventChannel<PanelPlacePortalEvent>
{

}

[CreateAssetMenu(fileName = "PanelTilesEventChannel", menuName = "EventChannels/PanelTilesEventChannel")]
public class PanelTilesEventChannel : ScriptableObject
{
    public PanelPlacePortalEventChannel OnPanelPlacePortal { get; private set; } = new PanelPlacePortalEventChannel();
}
