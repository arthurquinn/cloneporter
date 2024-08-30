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
    private Vector2 _portalEnterCheck;

    private Vector2 _lastPosition;

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
        // TODO: Not getting collision if both inside?? check this
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
            EnableCollisions(_ignoreCollisionsInPortal);
        }

        // Check portal entry
        _portalEnterCheck.x = Mathf.Max(0.01f, Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime);
        _portalEnterCheck.y = Mathf.Max(0.01f, Mathf.Abs(_rb.velocity.y) * Time.fixedDeltaTime);
        Collider2D collision = Physics2D.OverlapBox(transform.position, _portalEnterCheck, 0.0f, _portalLayer);
        if (collision != null && !_wasPorted)
        {
            IPortal portal = collision.GetComponent<IPortal>();
            if (portal != null)
            {
                portal.Port(_rb, _lastPosition);
                _wasPorted = true;
            }
        }
        else if (collision == null)
        {
            _wasPorted = false;
        }
        _lastPosition = _rb.position;
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _portalEnterCheck);
    }

    #endregion
}
