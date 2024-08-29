using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlayerPortalInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask _portalLayer;
    [SerializeField] private LayerMask _ignoreCollisionsInPortal;

    // Components
    private CapsuleCollider2D _collider;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    // Collision detections
    private float _rayPortalCheckVert;
    private float _rayPortalCheckHoriz;

    // Trackers
    private bool _wasPorted;


    private void Start()
    {
        _collider = GetComponent<CapsuleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // Calculate raycast lengths
        _rayPortalCheckVert = _spriteRenderer.bounds.extents.y + Mathf.Abs(_rb.velocity.y) * Time.fixedDeltaTime;
        _rayPortalCheckHoriz = _spriteRenderer.bounds.extents.x + Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime;

        // Check raycast hits
        bool didHitPortal = CheckPortalRaycast(Vector2.up, _rayPortalCheckVert) ||
            CheckPortalRaycast(Vector2.down, _rayPortalCheckVert) ||
            CheckPortalRaycast(Vector2.left, _rayPortalCheckHoriz) ||
            CheckPortalRaycast(Vector2.right, _rayPortalCheckHoriz);

        // Set collision state
        if (didHitPortal)
        {
            DisableCollisions(_ignoreCollisionsInPortal);
        }
        else
        {
            _wasPorted = false;
            EnableCollisions(_ignoreCollisionsInPortal);
        }

        // Check portal entry
        Collider2D collision = Physics2D.OverlapPoint(transform.position, _portalLayer);
        if (collision != null)
        {
            IPortal portal = collision.GetComponent<IPortal>();
            if (!_wasPorted)
            {
                portal.Port(_rb);
                _wasPorted = true;
            }
        }
    }

    private bool CheckPortalRaycast(Vector2 direction, float distance)
    {
        return Physics2D.Raycast(transform.position, direction, distance, _portalLayer).collider != null;
    }

    private void EnableCollisions(LayerMask collisionMask)
    {
        // Turn on collision for specified ignore layers (i.e. stop ignoring them)
        LayerMask current = Physics2D.GetLayerCollisionMask(gameObject.layer);
        LayerMask newMask = collisionMask | current;
        Physics2D.SetLayerCollisionMask(gameObject.layer, newMask);
    }

    private void DisableCollisions(LayerMask collisionMask)
    {
        // Turn off collision for specified ignore layers
        LayerMask current = Physics2D.GetLayerCollisionMask(gameObject.layer);
        LayerMask newMask = ~collisionMask & current;
        Physics2D.SetLayerCollisionMask(gameObject.layer, newMask);
    }


    #region Editor Methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.up * _rayPortalCheckVert));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.down * _rayPortalCheckVert));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.left * _rayPortalCheckHoriz));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.right * _rayPortalCheckHoriz));
    }

    #endregion
}
