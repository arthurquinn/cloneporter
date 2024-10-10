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

    [Header("Camera Follow Point")]
    [Tooltip("The camera follow point for the player is controlled by where the player is aiming.")]
    [SerializeField] private Transform _cameraPoint;

    private const float RAYCAST_LENGTH = 100.0f;
    private const float MIN_IK_DISTANCE = 0.5f;
    // Do not calculate targeting updates if the player is aiming too close into their body
    private const float MIN_IK_NO_CALC_DISTANCE = 0.1f;
    // This is needed to go "into" the tile a little but so we can retrieve it in the tilemap
    private const float HIT_DETECTION_MULTIPLIER = 0.01f;
    // This will help us extend our reflection raycast slightly so we don't hit the same tile
    //   we just reflected off of when calculating the next hit
    private const float RAYCAST_REFLECTION_OFFSET = 0.05f;

    // How fast the camera moves to follow the targeting beam
    private const float CAMERA_UPDATE_SPEED = 3f;
    // The threshold at which the camera can stop updating
    private const float CAMERA_MIN_UPDATE_DIST = 1f;

    private PlayerInputActions _inputs;

    private bool _isAiming;
    private Vector2 _aimTarget;
    private Vector2 _aimDirection;
    private RaycastHit2D _lastHit;

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

                // Camera point of the player will be where they are targeting
                SetCameraPoint(targetPosition);

                // Cache aim target and direction for when player fires
                _aimTarget = targetPosition;
                _aimDirection = aimDirection;
                _lastHit = hit;

                // Handle reflections if we hit a reflect tile
                if (hit.collider.CompareTag("Reflect"))
                {
                    HandleReflections(hit);
                }
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

    private void HandleReflections(RaycastHit2D hit)
    {
        // Initialize loop variables
        bool isReflecting = true;
        Vector2 reflectStart = _aimTarget;
        Vector2 reflectDir = Vector2.Reflect(_aimDirection, hit.normal);

        // Loop for all possible reflections
        while (isReflecting)
        {
            // Offset the reflect start slightly to avoid hitting the reflect tile we are bouncing off of
            reflectStart = reflectStart + (reflectDir * RAYCAST_REFLECTION_OFFSET);

            // Raycast to find next collider
            RaycastHit2D nextHit = Physics2D.Raycast(reflectStart, reflectDir, RAYCAST_LENGTH, _targetingLayers);
            if (nextHit.collider != null && nextHit.collider.CompareTag("Reflect"))
            {
                // Add the reflection point to the targeting beam
                AddToTargetingBeam(nextHit.point);

                // Set next reflection variables and continue
                reflectStart = nextHit.point;
                reflectDir = Vector2.Reflect(reflectDir, nextHit.normal);
            }
            else if (nextHit.collider != null)
            {
                // Add the reflection point to the targeting beam
                AddToTargetingBeam(nextHit.point);

                // Cache the new aim direction and aim target after all reflections for when player fires
                _aimTarget = nextHit.point;
                _aimDirection = reflectDir;
                _lastHit = nextHit;

                // End the reflections here and set target variables
                isReflecting = false;
            }
            else
            {
                // Didn't collide, so end reflection
                isReflecting = false;
            }
        }
    }

    private void SetCameraPoint(Vector2 point)
    {
        // Get the start and end position
        Vector2 currentPosition = _cameraPoint.position;
        Vector2 targetPosition = point;

        // Do not update for insignificant distances
        float distance = (targetPosition - currentPosition).magnitude;
        if (distance > CAMERA_MIN_UPDATE_DIST)
        {
            // Lerp to end position
            Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, CAMERA_UPDATE_SPEED * Time.deltaTime);

            // Set camera point to new position
            _cameraPoint.position = newPosition;
        }
    }

    private void SetGunTarget(Vector2 targetPosition)
    {
        _gunTarget.position = targetPosition;
    }

    private void AddToTargetingBeam(Vector2 targetPosition)
    {
        int linePos = _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(linePos, targetPosition);
    }

    private void DrawTargetingBeam(Vector2 targetPosition)
    {
        // Enable the line renderer
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 2;

        // Set the positions
        Vector2 gunPosition = (Vector2)_gunEffector.position;
        _lineRenderer.SetPosition(0, gunPosition);
        _lineRenderer.SetPosition(1, targetPosition);

        // Debug line
        Debug.DrawLine(_gunEffector.position, targetPosition);
    }

    private void ResetWeapon()
    {
        // Disable targeting beam
        _lineRenderer.enabled = false;

        // Reset weapon to reset position
        _gunTarget.position = _weaponRestPosition.position;

        // Reset the camera position
        SetCameraPoint(transform.position);
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
        // Check that we hit something
        if (_lastHit.collider != null)
        {
            // Check what we hit
            bool hitPortalTile = _lastHit.collider.CompareTag("Panels");
            bool hitPortal = _lastHit.collider.CompareTag("Portal");

            // If we hit a portal or a portal tile
            if (hitPortalTile || hitPortal)
            {
                // Get the entry point and direction
                // TODO: Find a way to not need the hit detection multiplier
                Vector2 entryPoint = _lastHit.point - (_lastHit.normal * HIT_DETECTION_MULTIPLIER);
                Vector2 entryDirection = _aimDirection;

                // Set up the entry ray of the fired shot
                Ray2D entry = new Ray2D(entryPoint, entryDirection);

                // Call unity event for portal gun fired
                _playerEvents.OnPortalGunFired.Raise(new PlayerPortalGunFireEvent(portalColor, entry));
            }
        }

    }
}
