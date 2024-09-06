using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BurnoutController : MonoBehaviour
{
    [Header("Laser Stats")]
    [SerializeField] private Transform _laserOriginPosition;
    [SerializeField] private float _maxLaserDistance;
    [SerializeField] private LayerMask _laserCollisionMask;

    [Header("Events")]
    [SerializeField] private UnityEvent<Vector2[], int[]> _onLaserPositionsChanged;

    private const int MAX_LASER_POSITIONS = 16;
    private Vector2[] _laserPositions;
    private int[] _laserSegments;

    private Vector2 _orientation;

    private void Awake()
    {
        _laserPositions = new Vector2[MAX_LASER_POSITIONS];
        _laserSegments = new int[MAX_LASER_POSITIONS * 2];
    }

    private void Start()
    {
        // Set our orientation based on our rotation
        _orientation = transform.rotation * Vector2.right;
    }

    // TODO: Lots of room to optimize here
    private void FixedUpdate()
    {
        // Clear out old laser positions and segments
        ClearPositions();
        ClearSegments();

        // Trace the laser path through reflections and ports through portals
        TracePath();

        // TODO: Make it so that this event is only called if our positions or segments are different
        _onLaserPositionsChanged.Invoke(_laserPositions, _laserSegments);
    }

    private void TracePath()
    {
        int segmentIndex = 0;
        int positionIndex = 0;

        // Initialize our first position
        Vector2 segmentOrigin = _laserOriginPosition.position;
        _laserSegments[segmentIndex++] = positionIndex;
        _laserPositions[positionIndex++] = segmentOrigin;


        // Iterate over all possible positions in our line
        while (segmentIndex < MAX_LASER_POSITIONS * 2)
        {
            // Check our first collision from a ray shot out directly in our orientation direction
            RaycastHit2D hit = Physics2D.Raycast(segmentOrigin, _orientation, _maxLaserDistance, _laserCollisionMask);
            if (hit.collider != null)
            {
                // If we get a hit, add the position to our laser
                _laserSegments[segmentIndex++] = positionIndex;
                _laserPositions[positionIndex++] = hit.point;

                // Check if the surface we hit is a portal we can teleport through
                IPortal portal = hit.collider.GetComponent<IPortal>();
                if (portal != null)
                {
                    // Get the exit ray from the linked portal
                    Ray2D entry = new Ray2D(hit.point, (hit.point - segmentOrigin).normalized);
                    Ray2D exit = portal.SimulatePort(entry);

                    // Set our exit origin as the beginning of a new line segment
                    _laserSegments[segmentIndex++] = positionIndex;
                    _laserPositions[positionIndex++] = exit.origin;
                    segmentOrigin = exit.origin;
                }

                // TODO: Check reflections ... remember to set the correct segment value

                // If we cannot teleport or reflect then our line has reached its terminus
                break;
            }
            else
            {
                // Technically this shouldn't happen if I'm designing levels correctly
                // I'll leave a warning here to see if I need to revisit this
                Debug.LogWarning("Laser did not reach terminal surface");
                break;
            }
        }
    }

    private void ClearPositions()
    {
        for (int i = 0; i < MAX_LASER_POSITIONS; i++)
        {
            _laserPositions[i] = Vector2.zero;
        }
    }

    private void ClearSegments()
    {
        for (int i = 0; i < MAX_LASER_POSITIONS * 2; i++)
        {
            _laserSegments[i] = -1;
        }
    }

    private void SetLaserPositions()
    {
        //_laserPositions[0] = _laserOriginPosition.position;
        //RaycastHit2D hit = Physics2D.Raycast(_laserOriginPosition.position, Vector2.right, _maxLaserDistance, _laserCollisionMask);
        //if (hit.collider != null)
        //{
        //    _laserPositions[1] = hit.point;
        //    IPortal portal = hit.collider.GetComponent<IPortal>();
        //    if (portal != null)
        //    {
        //        // Create a new ray defining the point the portal was hit, and the direction it is passing
        //        //   through the portal
        //        Vector2 inDirection = hit.point - (Vector2)_laserOriginPosition.position;
        //        Ray2D inRay = new Ray2D(hit.point, inDirection);

        //        // Get the output ray of the linked portal
        //        Ray2D outRay = portal.SimulatePort(inRay);

        //    }
        //}
    }
}
