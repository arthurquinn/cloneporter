using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BurnoutController : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("References")]
    [Tooltip("The transform that represents the origin of our laser.")]
    [SerializeField] private Transform _laserOrigin;

    [Header("Laser Stats")]
    [SerializeField] private float _maxLaserDistance;
    [SerializeField] private LayerMask _laserCollisionMask;

    [Header("Events")]
    [SerializeField] private UnityEvent<Vector3[][]> _onLaserPositionsChanged;

    [Header("Attack")]
    [SerializeField] private EnemyAttack _attackStats;

    private Vector2 _orientation;

    private const int MAX_LINES = 2; // Since there is only one portal pair, we can have a max of two lines
    private const int MAX_POSITIONS = 8; // For now assume a maximum of 8 bounces once we implement reflections
    private Vector3[][] _laserLines;

    private HealthController _hpController;

    private void Awake()
    {
        _laserLines = new Vector3[][]
        {
            new Vector3[MAX_POSITIONS],
            new Vector3[MAX_POSITIONS],
        };

        _hpController = GetComponent<HealthController>();
    }

    private void Start()
    {
        // Set our orientation based on our rotation
        _orientation = transform.rotation * Vector2.right;
        
        // Handle flaoting point precision errors
        _orientation.x = Mathf.Round(_orientation.x);
        _orientation.y = Mathf.Round(_orientation.y);
        _orientation = _orientation.normalized;
    }

    private void OnEnable()
    {
        // Listen for death event
        _hpController.OnDeath += HandleDeath;
        _playerEvents.OnDeath.Subscribe(HandlePlayerDeath);
    }

    private void OnDisable()
    {
        // Unhook deathevent
        _hpController.OnDeath -= HandleDeath;
        _playerEvents.OnDeath.Unsubscribe(HandlePlayerDeath);
    }

    // TODO: Lots of room to optimize here
    private void FixedUpdate()
    {
        // Update our orientation
        _orientation = transform.rotation * Vector2.right;

        // Clear out old laser positions
        ClearPositions();

        // Trace the laser path through reflections and ports through portals
        TracePath();

        // TODO: Make it so that this event is only called if our positions or segments are different
        _onLaserPositionsChanged.Invoke(_laserLines);
    }

    private void TracePath()
    {
        // Initialize line origin as the laser origin position
        Vector2 lineOrigin = _laserOrigin.position;
        Vector2 lineOrientation = _orientation;

        // Iterate over all possible lines
        for (int currentLine = 0; currentLine < MAX_LINES; currentLine++)
        {
            // Start of line is always the origin
            _laserLines[currentLine][0] = lineOrigin;
            for (int currentPosition = 1; currentPosition < MAX_POSITIONS;)
            {
                // Check if we hit a collid
                bool initialLine = currentLine == 0;
                RaycastHit2D hit = RaycastLaser(lineOrigin, lineOrientation, _laserCollisionMask, initialLine);
                Debug.DrawRay(lineOrigin, lineOrientation, Color.red, 5.0f);
                if (hit.collider != null)
                {
                    // Add the hit point to the current line
                    _laserLines[currentLine][currentPosition++] = hit.point;

                    // Check if the surface we hit is a portal we can teleport through
                    IPortal portal = hit.collider.GetComponent<IPortal>();
                    if (portal != null)
                    {
                        // Get the exit ray from the linked portal
                        Ray2D entry = new Ray2D(hit.point, ((Vector3)hit.point - _laserLines[currentLine][currentPosition - 2]).normalized);
                        Ray2D exit = portal.SimulatePort(entry);

                        // Set our exit origin as the beginning of a new line segment
                        lineOrigin = exit.origin;
                        lineOrientation = exit.direction;
                        break;
                    }

                    // Check if we hit a player
                    IAttackable attackable = hit.collider.GetComponent<IAttackable>();
                    if (attackable != null)
                    {
                        Ray2D laserRay = new Ray2D(lineOrigin, lineOrientation);
                        attackable.LaserAttack(_attackStats, laserRay);
                    }


                    // TODO: Check for surface reflections

                    // The laser has reached its terminus
                    return;
                }
                else
                {
                    // This shouldn't happen unless we have a level set up where a laser position is more
                    //   than the max distance from the nearest collider
                    Debug.LogWarning("Laser did not find terminal position");
                    return;
                }
            } 
        }
    }

    private RaycastHit2D RaycastLaser(Vector2 origin, Vector2 direction, LayerMask layerMask, bool initialLaser)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, _maxLaserDistance, layerMask);

        // If this is our first laser, ignore the first collision (which will be with this burnout object itself
        if (initialLaser && hits.Length > 1)
        {
            return hits[1];
        }

        // TODO: Make it so that we offset a small amount outside the portal and don't need to ignore here
        //// If this is our second laser then ignore the first portal collision (laser will be slightly inside portal)
        //if (!initialLaser && hits.Length > 1)
        //{
        //    return hits[1];
        //}

        // Otherwise return the first hit if any
        if (hits.Length > 0)
        {
            return hits[0];
        }

        return new RaycastHit2D();
    }

    private void ClearPositions()
    {
        for (int i = 0; i < _laserLines.Length; i++)
        {
            for (int j = 0; j < _laserLines[i].Length; j++)
            {
                _laserLines[i][j] = Vector2.zero;
            }
        }
    }

    private void HandleDeath()
    {
        // Clear out laser lines
        ClearPositions();

        // Update the line renderers
        _onLaserPositionsChanged.Invoke(_laserLines);

        // Disable this script
        enabled = false;
    }

    private void HandlePlayerDeath(PlayerDeathEvent @event)
    {
        if (@event.State == PlayerDeathState.Started)
        {
            // Disable this script next fixed update
            // We want to draw a line through the player instead of stopping at the player
            //   for a cooler effect
            StartCoroutine(DisableNextFixedUpdate());
        }
    }

    private IEnumerator DisableNextFixedUpdate()
    {
        yield return null;
        yield return new WaitForFixedUpdate();

        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(_laserOrigin.position, transform.rotation * Vector2.right);
    }
}
