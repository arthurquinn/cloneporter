using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("References")]

    [Tooltip("An array of line renderer components representing our laser.")]
    [SerializeField] private LineRenderer[] _laserLines;

    [Tooltip("A reference to a transform that represents the laser terminal position.")]
    [SerializeField] private Transform _laserTerminus;

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
                // Draw the laser line
                DrawLine(currentLine);

                // Set the terminal position
                SetLaserTerminus(currentLine);
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
        int positionCount = CountPositions(currentLine);

        // Draw the line
        lineRenderer.enabled = true;
        lineRenderer.positionCount = positionCount;
        lineRenderer.SetPositions(positions);
    }

    private void SetLaserTerminus(int currentLine)
    {
        // Get the terminal position
        Vector3[] positions = _positions[currentLine];
        int positionCount = CountPositions(currentLine);
        Vector3 terminalPosition = positions[positionCount - 1];

        // Set the terminal position transform
        _laserTerminus.position = terminalPosition;
    }

    private int CountPositions(int currentLine)
    {
        // Get all positions
        Vector3[] positions = _positions[currentLine];

        // Count the number of positions
        int positionCount = 0;
        for (; positionCount < _positions.Length && positions[positionCount] != Vector3.zero; positionCount++) { }

        return positionCount;
    }

    public void SetLaserPositions(Vector3[][] positions)
    {
        // Set the positions to be drawn next update
        _positions = positions;
    }
}
