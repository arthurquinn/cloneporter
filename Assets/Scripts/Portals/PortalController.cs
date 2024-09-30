using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PortalColor
{
    Purple,
    Teal,
}

public interface IPortal
{
    public PortalColor Color { get; }
    Ray2D SimulatePort(Ray2D entry);
    void ApplyPort(Ray2D entry, Rigidbody2D rigidbody);
}

public class PortalController : MonoBehaviour, IPortal
{
    [SerializeField] private PortalEventChannel _portalEventChannel;
    [SerializeField] private PortalColor _portalColor;

    [Header("Exit Velocity Adjustments")]
    [SerializeField] private float _exitUpVelocityThreshold;

    private PortalController _linkedPortal;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private Vector2 _orientation;
    public Vector2 Orientation { get { return _orientation; } }

    public PortalColor Color { get {  return _portalColor; } }

    private float _portalLength;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        CachePortalLength();
    }

    private void OnEnable()
    {
        _portalEventChannel.OnPortalOpened.Subscribe(HandlePortalOpened);
    }

    private void OnDisable()
    {
        _portalEventChannel.OnPortalOpened.Unsubscribe(HandlePortalOpened);
    }

    public void SetLinkedPortal(PortalController linkedPortal)
    {
        _linkedPortal = linkedPortal;
    }

    #region Public Methods

    public Ray2D SimulatePort(Ray2D entry)
    {
        // Calculate out direction
        Vector2 outDirection = Vector2.Reflect(entry.direction, Orientation);

        // Calculate and adjust for different rotations between portals
        float angleDiff = Vector2.SignedAngle(Orientation, _linkedPortal.Orientation);
        Quaternion rotationDiff = Quaternion.Euler(0, 0, angleDiff);
        outDirection = rotationDiff * outDirection;

        // Calculate out position
        Vector2 offset = entry.origin - (Vector2)transform.position;
        offset = rotationDiff * offset;

        // Break the rules for opposite orientations for more fun gameplay
        offset = AdjustForOppositeOrientations(offset);

        Vector2 outPosition = (Vector2)_linkedPortal.transform.position + offset;

        // Return ray defining simulated port
        return new Ray2D(outPosition, outDirection);
    }

    public void ApplyPort(Ray2D entry, Rigidbody2D rigidbody)
    {
        // Get the exit ray
        Ray2D exitRay = SimulatePort(entry);

        // Apply the port position to our rigibody
        rigidbody.position = exitRay.origin;

        // Calculate the exit velocity and apply it to our rigidbody
        Vector2 exitVelocity = rigidbody.velocity.magnitude * exitRay.direction;

        // Handle minor adjustments for player experience (e.g. min velocity out of portal)
        Vector2 adjustedExitVelocity = ApplyExitForceAdjustments(exitRay, exitVelocity);

        // Apply the force
        Vector2 appliedForce = adjustedExitVelocity - rigidbody.velocity;
        rigidbody.AddForce(appliedForce, ForceMode2D.Impulse);
    }

    private void HandlePortalOpened(PortalOpenedEvent portalOpenedEvent)
    {
        SetPortal(portalOpenedEvent.Position, portalOpenedEvent.Orientation);
    }

    public void SetPortal(Vector2 position, Vector2 orientation)
    {
        _spriteRenderer.enabled = true;
        _boxCollider.enabled = true;
        transform.position = position;
        SetRotation(orientation);
        _orientation = orientation;
    }

    public void ClearPortal()
    {
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;


    }

    public float GetLength()
    {
        return _portalLength;
    }

    #endregion

    private Vector2 AdjustForOppositeOrientations(Vector2 offset)
    {
        // Adjust when the portals are vertical and opposite from each other
        // This will allow the player to walk smoothly between portals placed near the ground
        //   without porting to the top of the next portal
        bool portalsVertical = Orientation.x != 0 && _linkedPortal.Orientation.x != 0;
        bool adjustVerticals =  Orientation.x == -_linkedPortal.Orientation.x;
        if (portalsVertical && adjustVerticals)
        {
            offset.y = -offset.y;
        }

        return offset;
    }

    private Vector2 ApplyExitForceAdjustments(Ray2D exitRay, Vector2 currentForce)
    {
        Vector2 adjustedForce = currentForce;

        // Give the player an exit boost if they are exiting a portal upward and they are below a certain speed
        // This helps them to jump out of a portal if they entered at a slow speed
        if (_linkedPortal.Orientation == Vector2.up)
        {
            adjustedForce.y = Mathf.Max(_exitUpVelocityThreshold, adjustedForce.y);
        }

        return adjustedForce;
    }

    private void SetRotation(Vector2 orientation)
    {
        if (orientation == Vector2.up || orientation == Vector2.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void CachePortalLength()
    {
        _portalLength = _spriteRenderer.bounds.size.y;

        // This assumes both portals will be the same size (likely won't change)
        _portalEventChannel.OnPortalStarted.Raise(new PortalStartedEvent(_portalLength));
    }
}
