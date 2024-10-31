using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPortalable
{
    public TeleportStats Stats { get; }
    public Rigidbody2D Rigidbody { get; }
    public Collider2D Collider { get; }
}

[RequireComponent(typeof(Collider2D))]
public class TeleportTrigger : MonoBehaviour, IPortalable
{
    #region Interface Methods
    public TeleportStats Stats { get { return _teleportStats; } }
    public Rigidbody2D Rigidbody { get { return _rb; } }
    public Collider2D Collider { get { return _collider; } }
    #endregion

    [Header("Teleport Stats")]
    [Tooltip("Stats specific to this teleporting object.")]
    [SerializeField] private TeleportStats _teleportStats;

    [Space(20)]

    [Tooltip("The collier of the teleporting rigid body.")]
    [SerializeField] private Collider2D _collider;

    [Tooltip("The portal layer.")]
    [SerializeField] private LayerMask _portalLayer;

    public UnityAction OnTeleported { get; set; }

    private Rigidbody2D _rb;
    private BoxCollider2D _trigger;

    private Vector2 _lastFixedPosition;

    private bool _didTeleport;

    private void Start()
    {
        _rb = GetComponentInParent<Rigidbody2D>();
        _trigger = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // Set last fixed position
        _lastFixedPosition = _rb.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPortal portal = collision.GetComponent<IPortal>();
        if (portal != null && !_didTeleport)
        {
            HandleEnteredPortal(portal);
        }
    }

    private void HandleEnteredPortal(IPortal portal)
    {
        // Calculate the entry ray
        Vector2 entryDirection = _rb.velocity.normalized;
        Vector2 entryPoint = _lastFixedPosition;
        Ray2D entryRay = new Ray2D(entryPoint, entryDirection);

        // Set our did teleport flag
        _didTeleport = true;

        // Use portal interface to apply port to our rigidbody
        portal.ApplyPort(entryRay, this);

        // Raise the portal leave portal event after fixed update
        StartCoroutine(RaiseTeleportedEvent());

        // Start coroutine to check when we left exit portal
        StartCoroutine(WaitForExitPortal(portal.Color));
    }

    private IEnumerator WaitForExitPortal(PortalColor enterColor)
    {
        // Convert our bounds into a square for better checking
        float maxSide = Mathf.Max(_trigger.bounds.size.x, _trigger.bounds.size.y);
        Vector2 boxSize = new Vector2(maxSide, maxSide);

        // Delay a frame
        yield return null;

        while (_didTeleport)
        {
            // Check each fixed update
            yield return new WaitForFixedUpdate();

            if (CheckDidExitPortal(boxSize, enterColor))
            {
                // If we exited the portal set our flag to false
                _didTeleport = false;
            }
        }
    }

    private bool CheckDidExitPortal(Vector2 boxSize, PortalColor enterColor)
    {
        // Check all portal overlaps
        // This is important for when portals are placed very close together
        // We want to consider touching a portal of an opposite color (but not the same color) as fully exited
        // Any instance of overlapping a portal of the entry color we will consider ourselves not exited
        Collider2D[] allOverlaps = Physics2D.OverlapBoxAll(_trigger.bounds.center, boxSize, 0, _portalLayer);
        for (int i = 0; i < allOverlaps.Length; i++)
        {
            // If the portal we are overlapping is the same as the enter color
            // Then we are not fully exited
            Collider2D overlap = allOverlaps[i];
            IPortal portal = overlap.GetComponent<IPortal>();
            if (portal.Color == enterColor)
            {
                return false;
            }
        }

        // We are not overlapping a portal of the enter color
        return true;
    }

    private IEnumerator RaiseTeleportedEvent()
    {
        yield return new WaitForFixedUpdate();
        if (OnTeleported != null)
        {
            OnTeleported();
        }
    }
}
