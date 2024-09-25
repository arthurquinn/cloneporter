using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


[CreateAssetMenu(fileName = "PlayerEventChannel", menuName = "EventChannels/PlayerEventChannel")]
public class PlayerEventChannel : ScriptableObject
{
    public PlayerPortalGunFireEventChannel OnPortalGunFired {  get; private set; } = new PlayerPortalGunFireEventChannel();
}
