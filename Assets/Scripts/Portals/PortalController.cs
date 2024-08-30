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
        // Port to new position
        Vector2 offset = fromPosition - (Vector2)transform.position;
        Vector2 portPosition = (Vector2)_linkedPortal.transform.position + offset;
        rigidbody.position = portPosition;

        // Determine new velocity
        Vector2 portVelocity = rigidbody.velocity;
        if (_orientation == _linkedPortal.Orientation)
        {
            portVelocity = Vector2.Reflect(rigidbody.velocity, -_orientation);
        }
        rigidbody.AddForce(portVelocity - rigidbody.velocity, ForceMode2D.Impulse);
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
