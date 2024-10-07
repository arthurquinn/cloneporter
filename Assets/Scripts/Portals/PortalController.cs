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
    void ApplyPort(Ray2D entry, Rigidbody2D rigidbody, Bounds bounds);
}

public class PortalController : MonoBehaviour, IPortal
{
    private const float ROUNDING_THRESHOLD = 0.001f;

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

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetLinkedPortal(PortalController linkedPortal)
    {
        _linkedPortal = linkedPortal;
    }

    #region Public Methods

    private Ray2D GetExitRay(Ray2D entry)
    {
        // Calculate out direction
        Vector2 outDirection = Vector2.Reflect(entry.direction, Orientation);

        // Calculate and adjust for different rotations between portals
        float angleDiff = Vector2.SignedAngle(Orientation, _linkedPortal.Orientation);
        Quaternion rotationDiff = Quaternion.Euler(0, 0, angleDiff);
        outDirection = rotationDiff * outDirection;

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

    public void ApplyPort(Ray2D entry, Rigidbody2D rigidbody, Bounds bounds)
    {
        // Get the exit ray
        Ray2D exitRay = GetExitRay(entry);

        // Break the rules for opposite orientations for more fun gameplay
        Vector2 offset = ApplyOffsetAdjustments(exitRay.origin, bounds);

        // Get the out position
        Vector2 outPosition = (Vector2)_linkedPortal.transform.position + offset;

        // Apply the port position to our rigibody
        rigidbody.position = outPosition;

        // Calculate the exit velocity and apply it to our rigidbody
        Vector2 exitVelocity = rigidbody.velocity.magnitude * exitRay.direction;

        // Handle minor adjustments for player experience (e.g. min velocity out of portal)
        Vector2 adjustedExitVelocity = ApplyExitForceAdjustments(exitRay, exitVelocity);

        // Apply the force
        Vector2 appliedForce = adjustedExitVelocity - rigidbody.velocity;
        rigidbody.AddForce(appliedForce, ForceMode2D.Impulse);
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

    public void CachePortalLength()
    {
        _portalLength = _spriteRenderer.bounds.size.y;
    }
}
