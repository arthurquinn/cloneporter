using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private LineRenderer[] _laserLines;

    private Vector3[][] _positions;

    private void Update()
    {
        DrawLines();
    }

    private void DrawLines()
    {
        for (int currentLine = 0; currentLine < _laserLines.Length; currentLine++)
        {
            // If there exists a line to be drawn
            if (_positions[currentLine][0] != Vector3.zero)
            {
                DrawLine(currentLine);
            }
            else
            {
                _laserLines[currentLine].enabled = false;
            }
        }
    }

    private void DrawLine(int currentLine)
    {
        // Get line renderer and positions
        LineRenderer lineRenderer = _laserLines[currentLine];
        Vector3[] positions = _positions[currentLine];

        // Count the number of positions
        int positionCount = 0;
        for (; positionCount < _positions.Length && positions[positionCount] != Vector3.zero; positionCount++) { }

        // Draw the line
        lineRenderer.enabled = true;
        lineRenderer.positionCount = positionCount;
        lineRenderer.SetPositions(positions);
    }

    public void SetLaserPositions(Vector3[][] positions)
    {
        // Set the positions to be drawn next update
        _positions = positions;
    }
}
