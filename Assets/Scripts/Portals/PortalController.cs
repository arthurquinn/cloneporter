using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPortal
{
    void Port(Rigidbody2D rigidbody, Vector2 fromPosition);
}

public class PortalController : MonoBehaviour, IPortal
{
    [SerializeField] private PortalController _linkedPortal;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private Vector2 _orientation;
    public Vector2 Orientation { get { return _orientation; } }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {

    }

    public void Port(Rigidbody2D rigidbody, Vector2 fromPosition)
    {
        //// Port to new position
        //Vector2 offset = fromPosition - (Vector2)transform.position;
        //if (_orientation != _linkedPortal.Orientation)
        //{
        //    offset = Vector2.Reflect(offset, _orientation);
        //}
        //Vector2 portPosition = (Vector2)_linkedPortal.transform.position + offset;
        //rigidbody.position = portPosition;

        //// Port to new position
        //Vector2 offset = fromPosition - (Vector2)transform.position;
        //Debug.Log("OFFSET: " + offset);
        //if (_orientation == Vector2.up)
        //{
        //    offset = GetOrientationUpOffset(offset);
        //}
        //Vector2 portPosition = (Vector2)_linkedPortal.transform.position + offset;
        //Debug.Log("NEW OFFSET: " + (portPosition - (Vector2)_linkedPortal.transform.position));
        //rigidbody.position = portPosition;

        //// Determine new velocity
        //Vector2 portVelocity = rigidbody.velocity;
        //if (_orientation == Vector2.up)
        //{
        //    portVelocity = Vector2.Reflect(rigidbody.velocity, -_orientation);
        //}
        //rigidbody.AddForce(portVelocity - rigidbody.velocity, ForceMode2D.Impulse);

        Vector2 offset = fromPosition - (Vector2)transform.position;
        Vector2 velocity = rigidbody.velocity;
        if (_orientation == Vector2.up)
        {
            offset = GetOrientationUpOffset(offset);
            velocity = GetOrientationUpVelocity(velocity);
        }
        else if (_orientation == Vector2.down)
        {
            offset = GetOrientationDownOffset(offset);
            velocity = GetOrientationDownVelocity(velocity);
        }
        rigidbody.position = (Vector2)_linkedPortal.transform.position + offset;
        rigidbody.AddForce(velocity - rigidbody.velocity, ForceMode2D.Impulse);
    }

    // Verified working
    public Vector2 GetOrientationUpOffset(Vector2 offset)
    {
        if (_linkedPortal.Orientation == Vector2.up)
        {
            return new Vector2(offset.x, offset.y);
        }
        else if (_linkedPortal.Orientation == Vector2.down)
        {
            return new Vector2(-offset.x, -offset.y);
        }
        else if (_linkedPortal.Orientation == Vector2.left)
        {
            return new Vector2(-offset.y, offset.x);
        }
        else if (_linkedPortal.Orientation == Vector2.right)
        {
            return new Vector2(offset.y, -offset.x);
        }

        return offset;
    }

    // Verified working
    public Vector2 GetOrientationDownOffset(Vector2 offset)
    {
        if (_linkedPortal.Orientation == Vector2.up)
        {
            return new Vector2(-offset.x, -offset.y);
        }
        else if (_linkedPortal.Orientation == Vector2.down)
        {
            return new Vector2(offset.x, offset.y);
        }
        else if ( _linkedPortal.Orientation == Vector2.left)
        {
            return new Vector2(offset.y, -offset.x);
        }
        else if (_linkedPortal.Orientation == Vector2.right)
        {
            return new Vector2(-offset.y, offset.x);
        }

        return offset;
    }

    // Test more (likely correct since it mirrors down)
    public Vector2 GetOrientationUpVelocity(Vector2 velocity)
    {
        if (_linkedPortal.Orientation == Vector2.up)
        {
            return Vector2.Reflect(velocity, -_orientation);
        }
        else if (_linkedPortal.Orientation == Vector2.down)
        {
            return velocity;
        }
        else if (_linkedPortal.Orientation == Vector2.left)
        {
            return new Vector2(velocity.y, velocity.x);
        }
        else if (_linkedPortal.Orientation == Vector2.right)
        {
            return new Vector2(-velocity.y, -velocity.x);
        }

        return velocity;
    }

    // Verified correct
    public Vector2 GetOrientationDownVelocity(Vector2 velocity)
    {
        if (_linkedPortal.Orientation == Vector2.up)
        {
            return velocity;
        }
        else if (_linkedPortal.Orientation == Vector2.down)
        {
            return Vector2.Reflect(velocity, -_orientation);
        }
        else if (_linkedPortal.Orientation == Vector2.left)
        {
            return new Vector2(-velocity.y, -velocity.x);
        }
        else if (_linkedPortal.Orientation == Vector2.right)
        {
            return new Vector2(velocity.y, velocity.x);
        }

        return velocity;
    }

    public void SetPortal(PortalPlacement placement)
    {
        _spriteRenderer.enabled = true;
        _boxCollider.enabled = true;
        transform.position = placement.Position;
        transform.rotation = placement.Rotation;
        _orientation = placement.Orientation;
    }

    public void ClearPortal()
    {
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
    }
}
