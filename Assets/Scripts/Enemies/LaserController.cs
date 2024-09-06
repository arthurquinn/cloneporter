using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private LineRenderer[] _laserLines;

    private Vector2[] _positions;
    private int[] _segments;

    private void Update()
    {
        // Variable to track scan ahead for segment counting 
        int segScan = 1;

        // Which line renderer should we use
        int currentLine = 0;

        // Iterate over every possible position in our laser
        for (int positionIndex = 0; positionIndex < _positions.Length;)
        {
            // First, count out how many segments will make up this section of laser
            // We need to do this to set the positions in our line renderer
            int segCount = 1;
            while (_segments[segScan] == _segments[segScan + 1])
            {
                segScan += 2;
                segCount++;
            }

            // Set the count before setting up line (unfortunately this seems to be required)
            // The number of positions always equal to the number of segments + 1
            _laserLines[currentLine].positionCount = segCount + 1;
            for (; positionIndex < segCount + 1; positionIndex++)
            {
                _laserLines[currentLine].SetPosition(positionIndex, _positions[positionIndex]);
            }
        }
    }

    public void SetLaserPositions(Vector2[] positions, int[] segments)
    {
        // TODO: This is a really good spot to check to see if the values are equal and not redraw them
        _positions = positions;
        _segments = segments;
    }
}
