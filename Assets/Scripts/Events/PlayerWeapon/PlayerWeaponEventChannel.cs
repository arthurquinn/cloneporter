using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PortalGunFiredEvent
{
    public PortalColor Color {  get; private set; }
    public Ray2D Entry { get; private set; }

    public PortalGunFiredEvent(PortalColor color, Ray2D entry)
    {
        Color = color;
        Entry = entry;
    }
}

public class PortalGunFiredEventChannel : AbstractEventChannel<PortalGunFiredEvent>
{

}

public struct AimPositionChangedEvent
{
    public Vector2 Position { get; private set; }

    public AimPositionChangedEvent(Vector2 position)
    {
        Position = position;
    }
}

public class AimPositionChangedEventChannel : AbstractEventChannel<AimPositionChangedEvent>
{

}

public struct AimStoppedEvent
{

}

public class AimStoppedEventChannel : AbstractEventChannel<AimStoppedEvent>
{

}


[CreateAssetMenu(fileName = "PlayerWeaponEventChannel", menuName = "EventChannels/PlayerWeaponEventChannel")]
public class PlayerWeaponEventChannel : ScriptableObject
{
    public PortalGunFiredEventChannel OnPortalGunFired { get; private set; }
    public AimPositionChangedEventChannel OnAimPositionChanged { get; private set; }
    public AimStoppedEventChannel OnAimStopped { get; private set; }

    private void OnEnable()
    {
        if (OnPortalGunFired == null)
        {
            OnPortalGunFired = new PortalGunFiredEventChannel();
        }
        if (OnAimPositionChanged == null)
        {
            OnAimPositionChanged = new AimPositionChangedEventChannel();
        }
        if (OnAimStopped == null)
        {
            OnAimStopped = new AimStoppedEventChannel();
        }
    }
}
