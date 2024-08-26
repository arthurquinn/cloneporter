using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private PortalGroupController _portalGroup;

    [Header("Hit Detection")]
    [SerializeField] private float _raycastLength;
    [SerializeField] private float _hitDetectionMultiplier;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _targetLayer;

    [Header("Tilemap")]
    [SerializeField] private PortalTiles _portalTiles;

    private PlayerInputActions _inputs;

    private LineRenderer _lineRenderer;

    private Vector2 _aimDirection;

    private void Awake()
    {
        _inputs = new PlayerInputActions();
    }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Player.FireLeft.performed += OnFireLeftInput;
        _inputs.Player.FireRight.performed += OnFireRightInput;
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void Update()
    {
        // Get normalized vector from weapon to mouse world position
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(_inputs.Player.Look.ReadValue<Vector2>());
        _aimDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;

        // Raycast to find nearest collider
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _aimDirection, _raycastLength, _targetLayer);
        if (hit.collider != null)
        {
            // Draw line to nearest collider in mouse direction
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, Vector2.zero);
            _lineRenderer.SetPosition(1, _aimDirection * hit.distance * transform.parent.localScale);
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    private void OnFireLeftInput(InputAction.CallbackContext context)
    {
        // Optimization note - got rid of lambda here since lambdas that capture variables
        //   may cause heap allocation
        TryShootPortal(ShootPurplePortal);
    }

    private void OnFireRightInput(InputAction.CallbackContext context)
    {
        // Optimization note - got rid of lambda here since lambdas that capture variables
        //   may cause heap allocation
        TryShootPortal(ShootTealPortal);
    }

    private void TryShootPortal(Action<Vector2, Quaternion> shootFunc)
    {
        // Check if we are aiming at a portal tile
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _aimDirection, _raycastLength, _targetLayer);
        if (hit.collider != null && hit.collider.CompareTag("PortalTiles"))
        {
            // TODO: I really want something better than this
            Vector2 adjustedHitPoint = hit.point + (hit.point - (Vector2)transform.position).normalized * _hitDetectionMultiplier;

            // Check if we can place a portal at the adjusted hit point
            if (_portalTiles.CanPlacePortal(adjustedHitPoint))
            {
                // Get the position and rotation that are valid according to the portal tiles tilemap
                (Vector2 position, Quaternion rotation) = _portalTiles.GetPortalPlacement(adjustedHitPoint, transform.position);
                shootFunc(position, rotation);
            }
        }
    }

    private void ShootPurplePortal(Vector2 position, Quaternion rotation)
    {
        _portalGroup.SetPurplePortal(position, rotation);
    }

    private void ShootTealPortal(Vector2 position, Quaternion rotation)
    {
        _portalGroup.SetTealPortal(position, rotation);
    }

    #region Gizmo Methods

    private void OnDrawGizmosSelected()
    {

    }

    #endregion
}
