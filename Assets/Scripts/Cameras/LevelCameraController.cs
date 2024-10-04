using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCameraController : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private CameraEventChannel _cameraEvents;
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Space(20)]

    [Tooltip("The first camera pair that should be used on scene load.")]
    [SerializeField] private PlayerCameraController _initialCameras;

    private PlayerCameraController[] _playerCameras;

    private void Awake()
    {
        _playerCameras = GetComponentsInChildren<PlayerCameraController>();
    }

    private void OnEnable()
    {
        _playerEvents.OnPlayerStarted.Subscribe(HandlePlayerStarted);
        _cameraEvents.OnZoneEnter.Subscribe(HandleZoneEnter);
        _cameraEvents.OnZoneExit.Subscribe(HandleZoneExit);
    }

    private void OnDisable()
    {
        _playerEvents.OnPlayerStarted.Unsubscribe(HandlePlayerStarted);
        _cameraEvents.OnZoneEnter.Unsubscribe(HandleZoneEnter);   
        _cameraEvents.OnZoneExit.Unsubscribe(HandleZoneExit);
    }

    private void HandlePlayerStarted(PlayerStartedEvent @event)
    {
        CachePlayer(@event.Player);
        DisableAllCameras();

        _initialCameras.EnableCameras();
    }

    private void CachePlayer(Transform player)
    {
        for (int i = 0; i < _playerCameras.Length; i++)
        {
            PlayerCameraController camera = _playerCameras[i];
            camera.CachePlayer(player);
        }
    }

    private void DisableAllCameras()
    {
        for (int i = 0; i < _playerCameras.Length; i++)
        {
            PlayerCameraController camera = _playerCameras[i];
            camera.DisableCameras();
        }
    }

    private void HandleZoneEnter(CameraZoneEnterEvent @event)
    {
        Debug.Log("Enter: " + @event.Cameras.gameObject.name);
    }

    private void HandleZoneExit(CameraZoneExitEvent @event)
    {
        Debug.Log("Exit: " + @event.Cameras.gameObject.name);
    }
}
