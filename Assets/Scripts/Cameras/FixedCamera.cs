using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FixedCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CameraActiveZone _activeZone;

    private void Start()
    {
        _virtualCamera.enabled = false;
    }

    private void OnEnable()
    {
        _activeZone.OnZoneEnter += HandleZoneEnter;
        _activeZone.OnZoneExit += HandleZoneExit;
    }

    private void OnDisable()
    {
        _activeZone.OnZoneEnter -= HandleZoneEnter;
        _activeZone.OnZoneExit -= HandleZoneExit;
    }

    private void HandleZoneEnter()
    {
        _virtualCamera.enabled = true;
    }

    private void HandleZoneExit()
    {
        _virtualCamera.enabled = false;
    }
}
