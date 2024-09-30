using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStartedEvent
{
    public Transform Player { get; private set; }
    
    public PlayerStartedEvent(Transform player)
    {
        Player = player;
    }
}

public class PlayerStartedEventChannel : AbstractEventChannel<PlayerStartedEvent>
{

}

public struct PlayerPortalGunFireEvent
{
    public PortalColor PortalColor { get; private set; }
    public Ray2D AimRay { get; private set; }

    public PlayerPortalGunFireEvent(PortalColor portalColor, Ray2D aimRay)
    {
        PortalColor = portalColor;
        AimRay = aimRay;
    }
}

public class PlayerPortalGunFireEventChannel : AbstractEventChannel<PlayerPortalGunFireEvent>
{

}

public struct PlayerTeleportedEvent
{

}

public class PlayerTeleportedEventChannel : AbstractEventChannel<PlayerTeleportedEvent>
{

}

public struct PlayerPickupItemEvent
{
    public ICarryable Item { get; private set; }

    public PlayerPickupItemEvent(ICarryable item)
    {
        Item = item;
    }
}

public class PlayerPickupItemEventChannel : AbstractEventChannel<PlayerPickupItemEvent>
{

}

public struct PlayerDropItemEvent
{
    public ICarryable Item { get; private set; }

    public PlayerDropItemEvent(ICarryable item)
    {
        Item = item;
    }
}

public class PlayerDropItemEventChannel : AbstractEventChannel<PlayerDropItemEvent>
{

}

[CreateAssetMenu(fileName = "PlayerEventChannel", menuName = "EventChannels/PlayerEventChannel")]
public class PlayerEventChannel : ScriptableObject
{
    public PlayerStartedEventChannel OnPlayerStarted {  get; private set; } = new PlayerStartedEventChannel();
    public PlayerPortalGunFireEventChannel OnPortalGunFired { get; private set; } = new PlayerPortalGunFireEventChannel();
    public PlayerTeleportedEventChannel OnTeleported { get; private set; } = new PlayerTeleportedEventChannel();
    public PlayerPickupItemEventChannel OnPickupItem { get; private set; } = new PlayerPickupItemEventChannel();
    public PlayerDropItemEventChannel OnDropItem { get; private set; } = new PlayerDropItemEventChannel();
}
