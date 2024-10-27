using UnityEngine;

public enum PortalColor
{
    Purple,
    Teal,
}

public interface IPortal
{
    public PortalColor Color { get; }
    Ray2D SimulatePort(Ray2D entry);
    void ApplyPort(Ray2D entry, IPortalable target);
}

public class PortalController : MonoBehaviour, IPortal
{
    private const float ROUNDING_THRESHOLD = 0.001f;

    [Header("Event Channels")]
    [SerializeField] private BurnoutEventChannel _burnoutEvents;

    [Header("Materials")]
    [Tooltip("The base portal material for this portal.")]
    [SerializeField] private Material _portalMaterial;
    [Tooltip("The material applied to the portal when it is obstructed by a laser.")]
    [SerializeField] private Material _laserPortalMaterial;

    [Space(20)]
    [SerializeField] private PortalColor _portalColor;
    [SerializeField] private LayerMask _teleportTriggerLayer;

    private PortalController _linkedPortal;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private Vector2 _orientation;
    public Vector2 Orientation { get { return _orientation; } }
    public PortalColor Color { get {  return _portalColor; } }

    private float _portalLength;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        _burnoutEvents.OnPortalEvent.Subscribe(HandlePortalEvent);
    }

    private void OnDisable()
    {
        _burnoutEvents.OnPortalEvent.Unsubscribe(HandlePortalEvent);
    }

    private void HandlePortalEvent(BurnoutLaserPortalEvent @event)
    {
        if (@event.Type == BurnoutLaserPortalEventType.Enter)
        {
            SetTeleportActive(false);
        }
        else
        {
            SetTeleportActive(true);
        }
    }

    public void SetLinkedPortal(PortalController linkedPortal)
    {
        _linkedPortal = linkedPortal;
    }

    #region Public Methods

    private Ray2D GetExitRay(Ray2D entry)
    {
        // Calculate the out direction
        Debug.DrawRay(entry.origin, entry.direction, UnityEngine.Color.yellow, 10.0f);
        Vector2 outDirection = Vector2.Reflect(entry.direction, Orientation);
        Debug.DrawRay(entry.origin, outDirection, UnityEngine.Color.blue, 10.0f);


        // Calculate and adjust for different rotations between portals
        float angleDiff = Vector2.SignedAngle(Orientation, _linkedPortal.Orientation);
        Quaternion rotationDiff = Quaternion.Euler(0, 0, angleDiff);
        outDirection = rotationDiff* outDirection;

        // Round off insignificantly small numbers to cardinal directions
        // This is useful in the case of lasers not looking crooked when exiting portals
        outDirection.x = RoundInsignificantValues(outDirection.x);
        outDirection.y = RoundInsignificantValues(outDirection.y);

        // Calculate out position
        Vector2 offset = entry.origin - (Vector2)transform.position;
        offset = rotationDiff * offset;

        return new Ray2D(offset, outDirection);
    }

    private float RoundInsignificantValues(float value)
    {
        if (Mathf.Abs(value) < ROUNDING_THRESHOLD)
        {
            return 0;
        }
        if (1 - Mathf.Abs(value) < ROUNDING_THRESHOLD)
        {
            return Mathf.Sign(value);
        }
        return value;
    }

    public Ray2D SimulatePort(Ray2D entry)
    {
        // Get the exit ray
        Ray2D exitRay = GetExitRay(entry);

        // Apply the offset from exit ray
        Vector2 outPosition = (Vector2)_linkedPortal.transform.position + exitRay.origin;

        // Return simulated port
        return new Ray2D(outPosition, exitRay.direction);
    }

    public void ApplyPort(Ray2D entry, IPortalable target)
    {
        // Get the exit ray
        Ray2D exitRay = GetExitRay(entry);

        //Debug.DrawRay(entry.origin, entry.direction, UnityEngine.Color.yellow, 10.0f);

        // Break the rules for opposite orientations for more fun gameplay
        Vector2 offset = ApplyOffsetAdjustments(exitRay.origin, target.Collider.bounds);

        // Adjust exit ray for smoother movement between portals
        Ray2D adjustedExitRay = ApplyDirectionAdjustments(exitRay);

        // Get the out position
        Vector2 outPosition = (Vector2)_linkedPortal.transform.position + offset;

        Debug.DrawRay(outPosition, adjustedExitRay.direction, UnityEngine.Color.red, 10.0f);

        // Apply the port position to our rigibody
        target.Rigidbody.position = outPosition;

        // Calculate the exit velocity and apply it to our rigidbody
        Vector2 exitVelocity = target.Rigidbody.velocity.magnitude * adjustedExitRay.direction;

        // Handle minor adjustments for player experience (e.g. min velocity out of portal)
        Vector2 adjustedExitVelocity = ApplyExitForceAdjustments(adjustedExitRay, exitVelocity, target.Stats);

        // Apply the force
        Vector2 appliedForce = adjustedExitVelocity - target.Rigidbody.velocity;
        target.Rigidbody.AddForce(appliedForce, ForceMode2D.Impulse);
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

    private Ray2D ApplyDirectionAdjustments(Ray2D exitRay)
    {
        // Special case of opposite horizontal portals for better movement
        bool bothHorizontal = Orientation.y != 0 && _linkedPortal.Orientation.y != 0;
        bool opposite = Orientation.y != _linkedPortal.Orientation.y;
        if (bothHorizontal && opposite)
        {
            // In this case reflect again for better movement
            Vector2 newDirection = Vector2.Reflect(exitRay.direction, Vector2.right);
            return new Ray2D(exitRay.origin, newDirection);
        }

        // Otherwise do nothing
        return exitRay;
    }

    private Vector2 ApplyOffsetAdjustments(Vector2 offset, Bounds bounds)
    {
        offset = AdjustForOppositeOrientations(offset);
        offset = AdjustForBounds(offset, bounds);
        return offset;
    }

    private Vector2 AdjustForBounds(Vector2 offset, Bounds bounds)
    {
        // Don't do this for vertical portals
        if (_linkedPortal.Orientation.x != 0)
        {
            //float portalXLen = _linkedPortal._spriteRenderer.bounds.size.x;
            float portalYLen = _linkedPortal._spriteRenderer.bounds.size.y;

            //float maxOffsetX = (portalXLen - bounds.size.x) / 2;
            float maxOffsetY = (portalYLen - bounds.size.y) / 2;

            //maxOffsetX -= 0.05f;
            maxOffsetY -= 0.05f;

            //float signX = Mathf.Sign(offset.x);
            float signY = Mathf.Sign(offset.y);

            //float absX = Mathf.Abs(offset.x);
            float absY = Mathf.Abs(offset.y);

            //float adjustedX = Mathf.Min(absX, maxOffsetX) * signX;
            float adjustedY = Mathf.Min(absY, maxOffsetY) * signY;

            return new Vector2(offset.x, adjustedY);
        }
        return offset;
    }

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

    private Vector2 ApplyExitForceAdjustments(Ray2D exitRay, Vector2 currentForce, TeleportStats stats)
    {
        Vector2 adjustedForce = currentForce;

        // Give the player an exit boost if they are exiting a portal upward and they are below a certain speed
        // This helps them to jump out of a portal if they entered at a slow speed
        if (_linkedPortal.Orientation == Vector2.up)
        {
            adjustedForce.y = Mathf.Max(stats.MinExitVelocityUp, adjustedForce.y);
        }

        // Give the player a horizontal boost if they are exiting a vertically standing portal and they are below a certain speed
        // This helps them to get "pushed" out of the portal if they entered really slowly
        if (_linkedPortal.Orientation.x != 0)
        {
            float speed = Mathf.Abs(adjustedForce.x);
            float direction = Mathf.Sign(adjustedForce.x);
            adjustedForce.x = direction * Mathf.Max(stats.MinExitVelocitySide, speed);
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

    private void SetTeleportActive(bool active)
    {
        // Change the material
        _spriteRenderer.material = active ? _portalMaterial : _laserPortalMaterial;

        // If we are active, set the portal to a trigger
        _boxCollider.isTrigger = active;
    }

    public void CachePortalLength()
    {
        _portalLength = _spriteRenderer.bounds.size.y;
    }
}
