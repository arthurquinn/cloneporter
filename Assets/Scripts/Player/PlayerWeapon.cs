using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
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
        _inputs.Player.Fire.performed += OnFireInput;
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
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    private void OnFireInput(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _aimDirection, _raycastLength, _targetLayer);
        if (hit.collider != null)
        {
            // This collider is the entire tilemap collider (i.e. all portal tiles connected to that collider)
            IPortalTiles portalTiles = hit.collider.GetComponent<IPortalTiles>();
            if (portalTiles != null)
            {
                // TODO: Is there something better than this?
                Vector2 adjustedHitPoint = hit.point * _hitDetectionMultiplier;
                portalTiles.HighlightTile(adjustedHitPoint);
            }
        }
    }

    #region Gizmo Methods

    private void OnDrawGizmosSelected()
    {

    }

    #endregion
}
