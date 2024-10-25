using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStartedEvent
{
    public Transform Player { get; private set; }
    public Transform CameraPoint { get; private set; }
    
    public PlayerStartedEvent(Transform player, Transform cameraPoint)
    {
        Player = player;
        CameraPoint = cameraPoint;
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

public struct PlayerCompleteLevelEvent
{

}

public class PlayerCompleteLevelEventChannel : AbstractEventChannel<PlayerCompleteLevelEvent>
{

}

public struct PlayerAimEvent
{
    public Vector2 Target;

    public PlayerAimEvent(Vector2 target)
    {
        Target = target;
    }
}

public class PlayerAimEventChannel : AbstractEventChannel<PlayerAimEvent>
{

}

public struct PlayerHPChangeEvent
{
    public IHealthAccessor PlayerHP { get; private set; }

    public PlayerHPChangeEvent(IHealthAccessor playerHP)
    {
        PlayerHP = playerHP;
    }
}

public class PlayerHPChangeEventChannel : AbstractEventChannel<PlayerHPChangeEvent>
{

}

public enum PlayerDeathState
{
    Started,
    Completed
}

public struct PlayerDeathEvent
{
    public PlayerDeathState State { get; private set; }

    public PlayerDeathEvent(PlayerDeathState state)
    {
        State = state;
    }
}

public class PlayerDeathEventChannel : AbstractEventChannel<PlayerDeathEvent>
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
    public PlayerCompleteLevelEventChannel OnCompleteLevel { get; private set; } = new PlayerCompleteLevelEventChannel();
    public PlayerAimEventChannel OnAim { get; private set; } = new PlayerAimEventChannel();
    public PlayerHPChangeEventChannel OnHPChanged { get; private set; } = new PlayerHPChangeEventChannel();
    public PlayerDeathEventChannel OnDeath { get; private set; } = new PlayerDeathEventChannel();
}
