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

    // Hard code this for now
    private readonly Vector2 _normal = new Vector2(0, -1); 

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

        // Reflect to new velocity
        Vector2 portVelocity = Vector2.Reflect(rigidbody.velocity, _normal);
        rigidbody.AddForce(portVelocity - rigidbody.velocity, ForceMode2D.Impulse);
    }

    public void SetPortal(Vector2 position, Quaternion rotation)
    {
        _spriteRenderer.enabled = true;
        _boxCollider.enabled = true;
        transform.position = position;
        transform.rotation = rotation;
    }

    public void ClearPortal()
    {
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
    }
}
