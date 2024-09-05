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
    [SerializeField] private UnityEvent<Vector2[]> _onLaserPositionsChanged;

    private const int MAX_LASER_POSITIONS = 16;
    private Vector2[] _laserPositions;

    private void Awake()
    {
        _laserPositions = new Vector2[MAX_LASER_POSITIONS];
    }

    // TODO: Lots of room to optimize here
    private void FixedUpdate()
    {
        ClearLaserPositions();
        SetLaserPositions();
        
        // Ideally we will check to see if any positions changed, and then trigger the line update
        //   Until then just always call this event (bad)
        if (_onLaserPositionsChanged != null)
        {
            _onLaserPositionsChanged.Invoke(_laserPositions);
        }
    }

    private void ClearLaserPositions()
    {
        for (int i = 0; i < _laserPositions.Length; i++)
        {
            _laserPositions[i] = Vector2.zero;
        }
    }

    private void SetLaserPositions()
    {
        _laserPositions[0] = _laserOriginPosition.position;
        RaycastHit2D hit = Physics2D.Raycast(_laserOriginPosition.position, Vector2.right, _maxLaserDistance, _laserCollisionMask);
        if (hit.collider != null)
        {
            _laserPositions[1] = hit.point;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_laserOriginPosition.position, Vector2.right);
    }
}
