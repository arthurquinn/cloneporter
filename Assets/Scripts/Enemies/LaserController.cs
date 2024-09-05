using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserController : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetLaser(Vector2[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] == Vector2.zero)
            {
                break;
            }
            _lineRenderer.SetPosition(i, positions[i]);
        }
    }
}
