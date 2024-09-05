using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    // TODO: Don't use a List -- its so bad but I'm focusing on
    //   shader for now
    private List<Vector2> _laserPositions;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _laserPositions = new List<Vector2>();
    }

    // TODO: Potential optimization: Only udpate laser coordinates
    //   when position of tracked world objects change
    private void Update()
    {
        DrawLaser();
    }

    private void FixedUpdate()
    {
        SetLaserPositions();
    }

    private void DrawLaser()
    {
        int i = 0;
        foreach (Vector2 position in _laserPositions)
        {
            _lineRenderer.SetPosition(i++, position);
        }
    }

    private void SetLaserPositions()
    {
        // Clear old list
        _laserPositions.Clear();

        // First position is always the origin
        _laserPositions.Add(transform.position);

        // Add collision
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _laserStartDirection, _laserDistance, _laserInteractionLayers);
        if (hit.collider != null)
        {
            _laserPositions.Add(hit.point);
        }
    }
}
