using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("Gun Targeting Transforms")]
    [Tooltip("Aim Origin should be the transform of the base of the arm that remains stationary. (Basically his elbow).")]
    [SerializeField] private Transform _aimOrigin;
    [SerializeField] private Transform _gunEffector;
    [SerializeField] private Transform _gunTarget;
    [Tooltip("A transform representing the position the gun target will return to when not actively being aimed.")]
    [SerializeField] private Transform _weaponRestPosition;
    [SerializeField] private LayerMask _targetingLayers;

    [Space(20)]

    [SerializeField] private LineRenderer _lineRenderer;

    private const float RAYCAST_LENGTH = 100.0f;
    private const float MIN_IK_DISTANCE = 0.5f;
    // Do not calculate targeting updates if the player is aiming too close into their body
    private const float MIN_IK_NO_CALC_DISTANCE = 0.1f;
    // This is needed to go "into" the tile a little but so we can retrieve it in the tilemap
    private const float HIT_DETECTION_MULTIPLIER = 0.01f;

    private PlayerInputActions _inputs;

    private bool _isAiming;
    private Vector2 _aimTarget;
    private Vector2 _aimDirection;


    private void Awake()
    {
        _inputs = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Enable inputs
        _inputs.Player.Look.Enable();
        _inputs.Player.FireLeft.Enable();
        _inputs.Player.FireRight.Enable();

        // Attach left event handlers
        _inputs.Player.FireLeft.started += AimLeftStart;
        _inputs.Player.FireLeft.canceled += AimLeftEnd;

        // Attach right event handlers
        _inputs.Player.FireRight.started += AimRightStart;
        _inputs.Player.FireRight.canceled += AimRightEnd;
    }

    private void OnDisable()
    {    
        // Remove left event handlers
        _inputs.Player.FireLeft.started -= AimLeftStart;
        _inputs.Player.FireLeft.canceled -= AimLeftEnd;

        // Remove right event handlers
        _inputs.Player.FireRight.started -= AimRightStart;
        _inputs.Player.FireRight.canceled -= AimRightEnd;

        // Disable inputs
        _inputs.Player.Look.Disable();
        _inputs.Player.FireLeft.Disable();
        _inputs.Player.FireRight.Disable();
    }

    private void Update()
    {
        if (_isAiming)
        {
            Aim();
        }
        else
        {
            ResetWeapon();
        }
    }

    private void Aim()
    {
        // Calculate the aim direction
        Vector2 ikTarget = Camera.main.ScreenToWorldPoint(_inputs.Player.Look.ReadValue<Vector2>());
        Vector2 aimOrigin = _aimOrigin.position;
        Vector2 aimDirection = (ikTarget - aimOrigin).normalized;

        // Get a minimum distance target to avoid IK weirdness
        float ikDistance = Vector2.Distance(aimOrigin, ikTarget);
        if (ikDistance < MIN_IK_DISTANCE)
        {
            ikTarget = aimOrigin + (aimDirection * MIN_IK_DISTANCE);
        }

        // This will prevent updates if the player is aiming inside their body
        if (ikDistance > MIN_IK_NO_CALC_DISTANCE)
        {
            // Raycast to find nearest collider
            RaycastHit2D hit = Physics2D.Raycast(aimOrigin, aimDirection, RAYCAST_LENGTH, _targetingLayers);
            if (hit.collider != null)
            {
                // Calculate the target position
                Vector2 targetPosition = aimOrigin + (aimDirection * hit.distance);

                // Set the target for IK and draw the targeting beam
                SetGunTarget(ikTarget);
                DrawTargetingBeam(targetPosition);

                // Cache aim target and direction for when player fires
                _aimTarget = targetPosition;
                _aimDirection = aimDirection;
            }
            else
            {
                // If no collisions then reset cached values
                _aimTarget = Vector2.zero;
                _aimDirection = Vector2.zero;
            }
        }
        else
        {
            // Best to knock them out of aiming if they are literally aiming directly into their chest
            // Maybe I can revisit this but for now it will fix the IK weirdness
            ResetWeapon();
        }
    }

    private void SetGunTarget(Vector2 targetPosition)
    {
        _gunTarget.position = targetPosition;
    }

    private void DrawTargetingBeam(Vector2 targetPosition)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _gunEffector.position);
        _lineRenderer.SetPosition(1, targetPosition);
    }

    private void ResetWeapon()
    {
        // Disable targeting beam
        _lineRenderer.enabled = false;

        // Reset weapon to reset position
        _gunTarget.position = _weaponRestPosition.position;
    }

    // TODO: Would be cool to color lines differently based on aiming
    private void AimLeftStart(InputAction.CallbackContext context)
    {
        _isAiming = true;
    }

    private void AimLeftEnd(InputAction.CallbackContext context)
    {
        FirePortalGun(PortalColor.Purple);
    }

    private void AimRightStart(InputAction.CallbackContext context)
    {
        _isAiming = true;
    }

    private void AimRightEnd(InputAction.CallbackContext context)
    {
        FirePortalGun(PortalColor.Teal);
    }

    private void FirePortalGun(PortalColor portalColor)
    {
        // We are no longer aiming
        _isAiming = false;

        // If we are hitting a target
        if (_aimTarget != Vector2.zero && _aimDirection != Vector2.zero)
        {
            CheckValidCollision(portalColor);
        }
    }

    private void CheckValidCollision(PortalColor portalColor)
    {
        Vector2 aimOrigin = _aimOrigin.position;

        // Check if we are aiming at a portal tile
        RaycastHit2D hit = Physics2D.Raycast(aimOrigin, _aimDirection, RAYCAST_LENGTH, _targetingLayers);
        if (hit.collider != null)
        {
            // Check what we hit
            bool hitPortalTile = hit.collider.CompareTag("Panels");
            bool hitPortal = hit.collider.CompareTag("Portal");

            // If we hit a portal or a portal tile
            if (hitPortalTile || hitPortal)
            {
                // TODO: I really want something better than this
                Vector2 adjustedHitPoint = hit.point + ((hit.point - aimOrigin).normalized) * HIT_DETECTION_MULTIPLIER;
                Vector2 entryDirection = (adjustedHitPoint - aimOrigin).normalized;

                // Set up the entry ray of the fired shot
                Ray2D entry = new Ray2D(adjustedHitPoint, entryDirection);

                // Call unity event for portal gun fired
                _playerEvents.OnPortalGunFired.Raise(new PlayerPortalGunFireEvent(portalColor, entry));
            }
        }
    }

    //private void TryShootPortal(PortalColor portalColor)
    //{

    //}
}
