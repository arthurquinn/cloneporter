using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public enum PortalColor
{
    Purple,
    Teal,
}

public interface IPortal
{
    Ray2D SimulatePort(Ray2D entry);
    void ApplyPort(Ray2D entry, Rigidbody2D rigidbody);
}

public class PortalController : MonoBehaviour, IPortal
{
    [SerializeField] private PortalController _linkedPortal;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private Vector2 _orientation;
    public Vector2 Orientation { get { return _orientation; } }

    private float _portalLength;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        CachePortalLength();
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
        Vector2 appliedForce = exitVelocity - rigidbody.velocity;
        rigidbody.AddForce(appliedForce, ForceMode2D.Impulse);
    }

    public void SetPortal(PortalPlacement placement)
    {
        _spriteRenderer.enabled = true;
        _boxCollider.enabled = true;
        transform.position = placement.Position;
        SetRotation(placement.Orientation);
        _orientation = placement.Orientation;

        Debug.Log("Orientation: " + _orientation);
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
    }
}
