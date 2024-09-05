using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _laserDistance;
    [SerializeField] private Vector2 _laserStartDirection;
    [SerializeField] private LayerMask _laserInteractionLayers;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // TODO: Potential optimization: Only udpate laser coordinates
    //   when position of tracked world objects change
    private void Update()
    {
        SetLaserPositions();
    }

    private void SetLaserPositions()
    {
        int hitCount = 0;

        // First position is always the origin
        _lineRenderer.SetPosition(0, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _laserStartDirection, _laserDistance, _laserInteractionLayers);
        if (hit.collider != null)
        {
            _lineRenderer.SetPosition(++hitCount, hit.point);
        }
    }
}
