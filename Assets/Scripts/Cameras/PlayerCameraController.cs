using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    private enum CameraLabel
    {
        CameraA,
        CameraB
    }

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;
    [SerializeField] private CameraEventChannel _cameraEvents;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _playerCameraA;
    [SerializeField] private CinemachineVirtualCamera _playerCameraB;

    [Header("Bounding Shape")]
    [SerializeField] private CameraBoundingShape _boundingShape;

    private bool _isActive;

    private Transform _playerTransform;

    private CameraLabel _currentCamera;

    private void OnEnable()
    {
        _playerEvents.OnPlayerStarted.Subscribe(HandlePlayerStarted);
        _playerEvents.OnTeleported.Subscribe(HandlePlayerTeleported);

        _boundingShape.OnZoneEnter += HandleZoneEnter;
        _boundingShape.OnZoneExit += HandleZoneExit;
    }

    private void OnDisable()
    {
        _playerEvents.OnPlayerStarted.Unsubscribe(HandlePlayerStarted);
        _playerEvents.OnTeleported.Unsubscribe(HandlePlayerTeleported);

        _boundingShape.OnZoneEnter -= HandleZoneEnter;
        _boundingShape.OnZoneExit -= HandleZoneExit;
    }

    private void HandlePlayerStarted(PlayerStartedEvent @event)
    {
        // Cache the player transform
        _playerTransform = @event.Player;
    }

    private void HandlePlayerTeleported(PlayerTeleportedEvent @event)
    {
        // Switch the active camera when the player teleports
        // We have an ease in out blend between these two cameras so that the camera motion on teleport
        //   is not too jarring for the user
        StartCoroutine(SwitchCameraNextFixedUpdate());
    }

    private IEnumerator SwitchCameraNextFixedUpdate()
    {
        // Wait for next fixed update so we set the camera follow target after the player's rigidbody has updated
        // Then switch the camera
        yield return new WaitForFixedUpdate();

        // Do not switch camera if we are not active (would occur if player teleported into new zone)
        if (_isActive)
        {
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        if (_currentCamera == CameraLabel.CameraA)
        {
            UseCameraB();
        }
        else
        {
            UseCameraA();
        }
    }

    private void UseCameraA()
    {
        // Set the current camera label
        _currentCamera = CameraLabel.CameraA;

        // Set the priorities
        _playerCameraA.Priority = 10;
        _playerCameraB.Priority = 0;

        // Set and unset follow target
        _playerCameraA.Follow = _playerTransform;
        _playerCameraB.Follow = null;
    }

    private void UseCameraB()
    {
        // Set the current camera label
        _currentCamera = CameraLabel.CameraB;

        // Set the priorities
        _playerCameraB.Priority = 10;
        _playerCameraA.Priority = 0;

        // Set and unset follow target
        _playerCameraB.Follow = _playerTransform;
        _playerCameraA.Follow = null;
    }

    private void HandleZoneEnter()
    {
        EnableCameras();
    }

    private void HandleZoneExit()
    {
        DisableCameras();
    }

    public void EnableCameras()
    {
        // Start with camera A
        UseCameraA();

        // Set is active flag
        _isActive = true;
    }

    public void DisableCameras()
    {
        // Set priorities
        _playerCameraA.Priority = 0;
        _playerCameraB.Priority = 0;

        // Set is active flag
        _isActive = false;
    }
}
