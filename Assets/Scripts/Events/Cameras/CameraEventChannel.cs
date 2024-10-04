using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraZoneEnterEvent
{
    public PlayerCameraController Cameras;

    public CameraZoneEnterEvent(PlayerCameraController cameras)
    {
        Cameras = cameras;
    }
}

public class CameraZoneEnterEventChannel : AbstractEventChannel<CameraZoneEnterEvent>
{

}

public struct CameraZoneExitEvent
{
    public PlayerCameraController Cameras;

    public CameraZoneExitEvent(PlayerCameraController cameras)
    {
        Cameras = cameras;
    }
}

public class CameraZoneExitEventChannel : AbstractEventChannel<CameraZoneExitEvent>
{

}

[CreateAssetMenu(fileName = "CameraEventChannel", menuName = "EventChannels/CameraEventChannel")]
public class CameraEventChannel : ScriptableObject
{
    public CameraZoneEnterEventChannel OnZoneEnter {  get; private set; } = new CameraZoneEnterEventChannel();
    public CameraZoneExitEventChannel OnZoneExit { get; private set; } = new CameraZoneExitEventChannel();
}
