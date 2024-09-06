using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BurnoutController : MonoBehaviour
{
    [Header("Laser Stats")]
    [SerializeField] private Transform _laserOriginPosition;
    [SerializeField] private float _maxLaserDistance;
    [SerializeField] private LayerMask _laserCollisionMask;
    [SerializeField] private LayerMask _portalMask;

    [Header("Events")]
    [SerializeField] private UnityEvent<Vector3[][]> _onLaserPositionsChanged;

    private Vector2 _orientation;

    private const int MAX_LINES = 2;
    private const int MAX_POSITIONS = 8;
    private Vector3[][] _laserLines;

    private void Awake()
    {
        _laserLines = new Vector3[][]
        {
            new Vector3[MAX_POSITIONS],
            new Vector3[MAX_POSITIONS],
        };
    }

    private void Start()
    {
        // Set our orientation based on our rotation
        _orientation = transform.rotation * Vector2.right;
    }

    // TODO: Lots of room to optimize here
    private void FixedUpdate()
    {
        // Clear out old laser positions
        ClearPositions();

        // Trace the laser path through reflections and ports through portals
        TracePath();

        Debug.Log("Laser Lines: " + _laserLines);

        // TODO: Make it so that this event is only called if our positions or segments are different
        _onLaserPositionsChanged.Invoke(_laserLines);
    }

    private void TracePath()
    {
        // Initialize line origin as the laser origin position
        Vector2 lineOrigin = _laserOriginPosition.position;
        Vector2 lineOrientation = _orientation;

        // Iterate over all possible lines
        for (int currentLine = 0; currentLine < MAX_LINES; currentLine++)
        {
            // Start of line is always the origin
            _laserLines[currentLine][0] = lineOrigin;
            for (int currentPosition = 1; currentPosition < MAX_POSITIONS;)
            {
                // Remove portal from collision mask if this isn't our first line
                LayerMask layerMask = _laserCollisionMask;
                if (currentLine != 0)
                {
                    layerMask &= ~_portalMask;
                }

                // Check if we hit a collider
                RaycastHit2D hit = Physics2D.Raycast(lineOrigin, lineOrientation, _maxLaserDistance, layerMask);
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

                    // TODO: Check for surface reflections

                    // The laser has reached its terminus
                    return;
                }
                else
                {
                    // This shouldn't happen unless we have a level set up where a laser position is more
                    //   than the max distance from the nearest collider
                    Debug.LogWarning("Laser did not find terminal position");
                }
            } 
        }
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
}
