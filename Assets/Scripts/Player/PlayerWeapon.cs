using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent<Vector2, Vector2> _onPurplePortalFire;
    [SerializeField] private UnityEvent<Vector2, Vector2> _onTealPortalFire;

    [Header("Hit Detection")]
    [SerializeField] private float _raycastLength;
    [SerializeField] private float _hitDetectionMultiplier;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _targetLayer;

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
        TryShootPortal(_onPurplePortalFire);
    }

    private void OnFireRightInput(InputAction.CallbackContext context)
    {
        // Optimization note - got rid of lambda here since lambdas that capture variables
        //   may cause heap allocation
        TryShootPortal(_onTealPortalFire);
    }

    private void TryShootPortal(UnityEvent<Vector2, Vector2> shootEvent)
    {
        // Check if we are aiming at a portal tile
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _aimDirection, _raycastLength, _targetLayer);
        if (hit.collider != null && hit.collider.CompareTag("PortalTiles"))
        {
            // TODO: I really want something better than this
            Vector2 adjustedHitPoint = hit.point + (hit.point - (Vector2)transform.position).normalized * _hitDetectionMultiplier;

            // Invoke the unity event
            if (shootEvent != null)
            {
                shootEvent.Invoke(adjustedHitPoint, transform.position);
            }
        }
    }

    #region Gizmo Methods

    private void OnDrawGizmosSelected()
    {

    }

    #endregion
}
