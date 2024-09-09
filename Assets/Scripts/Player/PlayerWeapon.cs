using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class PlayerWeapon : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent<Vector2, Vector2> _onPurplePortalFire;
    [SerializeField] private UnityEvent<Vector2, Vector2> _onTealPortalFire;
    [SerializeField] private UnityEvent<Vector2> _onPlayerAimPosition;
    [SerializeField] private UnityEvent _onPlayerStopAim;

    [Header("Hit Detection")]
    [SerializeField] private float _raycastLength;
    [SerializeField] private float _hitDetectionMultiplier;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _targetLayer;

    [Header("Aim Target")]
    [SerializeField] private Transform _rightArmEffector;
    [SerializeField] private Transform _initialAimOrigin;

    private PlayerInputActions _inputs;
    private LineRenderer _lineRenderer;

    private bool _isAiming;
    private Vector2 _aimDirection;
    private Vector2 _aimOrigin;

    private void Awake()
    {
        _inputs = new PlayerInputActions();
    }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _aimOrigin = _initialAimOrigin.position;
    }

    private void OnEnable()
    {
        // Enable
        _inputs.Player.Look.Enable();
        _inputs.Player.FireLeft.Enable();
        _inputs.Player.FireRight.Enable();

        // Set up events
        _inputs.Player.FireLeft.started += OnFireLeftStart;
        _inputs.Player.FireRight.started += OnFireRightStart;
        _inputs.Player.FireLeft.canceled += OnFireLeftCanceled;
        _inputs.Player.FireRight.canceled += OnFireRightCanceled;
    }

    private void OnDisable()
    {
        // Tear down events
        _inputs.Player.FireLeft.started -= OnFireLeftStart;
        _inputs.Player.FireRight.started -= OnFireRightStart;
        _inputs.Player.FireLeft.canceled -= OnFireLeftCanceled;
        _inputs.Player.FireRight.canceled -= OnFireRightCanceled;

        // Disable
        _inputs.Player.Look.Disable();
        _inputs.Player.FireLeft.Disable();
        _inputs.Player.FireRight.Disable();
    }

    private void Update()
    {
        if (_isAiming)
        {
            DrawAimLine();
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    private void DrawAimLine()
    {
        // Get normalized vector from weapon to mouse world position
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(_inputs.Player.Look.ReadValue<Vector2>());
        _aimDirection = (mouseWorldPosition - _aimOrigin).normalized;

        // Raycast to find nearest collider
        RaycastHit2D hit = Physics2D.Raycast(_aimOrigin, _aimDirection, _raycastLength, _targetLayer);
        Debug.Log(hit);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider);

            // Draw line to nearest collider in mouse direction
            Vector2 aimPos = _aimOrigin + (_aimDirection * hit.distance);
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, _aimOrigin);
            _lineRenderer.SetPosition(1, aimPos);

            // Invoke our on aim event
            _onPlayerAimPosition.Invoke(aimPos);
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    // These methods are separated right now because I want to change color of left and right lines
    //   in the future
    private void OnFireLeftStart(InputAction.CallbackContext context)
    {
        _isAiming = true;
    }

    private void OnFireRightStart(InputAction.CallbackContext context)
    {
        _isAiming = true;
    }

    private void OnFireLeftCanceled(InputAction.CallbackContext context)
    {
        _isAiming = false;
        TryShootPortal(_onPurplePortalFire);

        _onPlayerStopAim.Invoke();
    }

    private void OnFireRightCanceled(InputAction.CallbackContext context)
    {
        _isAiming = false;
        TryShootPortal(_onTealPortalFire);

        _onPlayerStopAim.Invoke();
    }

    private void TryShootPortal(UnityEvent<Vector2, Vector2> shootEvent)
    {
        // Check if we are aiming at a portal tile
        RaycastHit2D hit = Physics2D.Raycast(_aimOrigin, _aimDirection, _raycastLength, _targetLayer);
        if (hit.collider != null && hit.collider.CompareTag("PortalTiles"))
        {
            // TODO: I really want something better than this
            Vector2 adjustedHitPoint = hit.point + ((hit.point - _aimOrigin).normalized) * _hitDetectionMultiplier;

            // Invoke the unity event
            if (shootEvent != null)
            {
                shootEvent.Invoke(adjustedHitPoint, _aimOrigin);
            }
        }
    }

    #region Public Event Handlers

    public void ChangeTargetLaserOrigin(Vector2 newOrigin)
    {
        _aimOrigin = newOrigin;
    }

    #endregion
}
