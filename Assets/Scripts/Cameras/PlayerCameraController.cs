using Cinemachine;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    private enum CameraLabel
    {
        CameraA,
        CameraB
    }

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _playerCameraA;
    [SerializeField] private CinemachineVirtualCamera _playerCameraB;

    private Transform _playerTransform;

    private CameraLabel _currentCamera;

    private void OnEnable()
    {
        _playerEvents.OnPlayerStarted.Subscribe(HandlePlayerStarted);
        _playerEvents.OnTeleported.Subscribe(HandlePlayerTeleported);
    }

    private void OnDisable()
    {
        _playerEvents.OnPlayerStarted.Unsubscribe(HandlePlayerStarted);
        _playerEvents.OnTeleported.Unsubscribe(HandlePlayerTeleported);
    }

    private void HandlePlayerStarted(PlayerStartedEvent @event)
    {
        // Cache the player transform
        _playerTransform = @event.Player;

        // Initialize the current camera with camera A
        UseCameraA();
    }

    private void HandlePlayerTeleported(PlayerTeleportedEvent @event)
    {
        // Switch the active camera when the player teleports
        // We have an ease in out blend between these two cameras so that the camera motion on teleport
        //   is not too jarring for the user
        SwitchCamera();
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
}
