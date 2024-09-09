using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public struct PortalPlacement
{
    public Vector2 Position { get; private set; }
    public Vector2 Orientation { get; private set; }

    public PortalPlacement(Vector2 position, Vector2 orientation)
    {
        Position = position;
        Orientation = orientation;
    }
}

public class PortalGround : MonoBehaviour
{
    [SerializeField] private Tilemap _groundTilemap;

    [SerializeField] private UnityEvent<PortalPlacement> _onPurplePortalOpened;
    [SerializeField] private UnityEvent<PortalPlacement> _onTealPortalOpened;

    private PortalController _portal;
    private Tilemap _tilemap;

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _portal = GetComponentInChildren<PortalController>();
    }

    public void OpenPortal(PortalColor color, Ray2D entry)
    {
        Vector3Int cellPosition = _tilemap.WorldToCell(entry.origin);
        if (_tilemap.HasTile(cellPosition))
        {
            if (CanOpenVertically(cellPosition, entry))
            {
                // Get the cell centered target location for the portal
                Vector2 cellCenterWorld = _tilemap.GetCellCenterWorld(cellPosition);
                Vector2 targetCentered = new Vector2(cellCenterWorld.x, entry.origin.y);

                OpenPortalForColor(color, targetCentered, GetVerticalOrientation(entry.direction));
            }
            else if (CanOpenHorizontally(cellPosition, entry))
            {
                // Get the cell centered target location for the portal
                Vector2 cellCenterWorld = _tilemap.GetCellCenterWorld(cellPosition);
                Vector2 targetCentered = new Vector2(entry.origin.x, cellCenterWorld.y);

                OpenPortalForColor(color, targetCentered, GetHorizontalOrientation(entry.direction));
            }
        }
    }

    private Vector2 GetVerticalOrientation(Vector2 entryDirection)
    {
        if (entryDirection.x > 0)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.right;
        }
    }

    private Vector2 GetHorizontalOrientation(Vector2 entryDirection)
    {
        if (entryDirection.y > 0)
        {
            return Vector2.down;
        }
        else
        {
            return Vector2.up;
        }
    }

    private void OpenPortalForColor(PortalColor color, Vector2 position, Vector2 orientation)
    {
        if (color == PortalColor.Purple)
        {
            _onPurplePortalOpened.Invoke(new PortalPlacement(position, orientation));
        }
        else if (color == PortalColor.Teal)
        {
            _onTealPortalOpened.Invoke(new PortalPlacement(position, orientation));
        }
        else
        {
            Debug.LogWarning("Unreachable code: Should have a portal color of teal or purple");
        }
    }

    private bool CanOpenHorizontally(Vector3Int cellPosition, Ray2D entry)
    {
        // Get the cell centered target location for the portal
        Vector2 cellCenterWorld = _tilemap.GetCellCenterWorld(cellPosition);
        Vector2 targetCentered = new Vector2(entry.origin.x, cellCenterWorld.y);

        DebugRayAtPosition(targetCentered, Color.red, Vector2.up);

        // Since we are placing the portal horizontally, multiply the y extent by the
        //   vector (1, 0) to get the horizontal length
        Bounds portalBounds = _portal.GetBounds();
        Vector2 checkPosition = targetCentered - portalBounds.extents.y * Vector2.right;
        Vector2 rightPosition = targetCentered + portalBounds.extents.y * Vector2.right;

        // If we shot down, the above tiles must be empty
        // If we shot up, the below tiles must be empty
        Vector3Int emptyDirection = GetHorizontalEmptyDirection(entry.direction);

        DebugRayAtPosition(checkPosition, Color.red, Vector2.up);
        DebugRayAtPosition(rightPosition, Color.red, Vector2.up);

        return CheckAllHorizontalPositions(checkPosition, rightPosition, emptyDirection);
    }

    private bool CheckAllHorizontalPositions(Vector2 checkPosition, Vector2 rightPosition, Vector3Int emptyDirection)
    {
        // Keep checking until we are past the right position by more than one cell size width
        bool canOpen = true;
        bool rightCheck = false;
        while (checkPosition.x < rightPosition.x || !rightCheck)
        {
            // If we are past the right position, snap the check to the right position
            if (checkPosition.x > rightPosition.x)
            {
                checkPosition.x = rightPosition.x;
                rightCheck = true;
            }

            // Get the cell position of our check position
            Vector3Int checkCellPosition = _tilemap.WorldToCell(checkPosition);

            // Is this a valid position to open a vertical portal?
            if (IsValidPosition(checkCellPosition, emptyDirection))
            {
                DebugRayAtPosition(checkPosition, Color.green, Vector2.up);
                checkPosition.x += _tilemap.cellSize.x;
            }
            else
            {
                canOpen = false;
                break;
            }
        }
        return canOpen;
    }

    private Vector3Int GetHorizontalEmptyDirection(Vector2 entryDirection)
    {
        Vector3Int emptyDirection;
        if (entryDirection.y > 0)
        {
            emptyDirection = Vector3Int.down;
        }
        else
        {
            emptyDirection = Vector3Int.up;
        }
        return emptyDirection;
    }

    private bool CanOpenVertically(Vector3Int cellPosition, Ray2D entry)
    {
        // Get the cell centered target location for the portal
        Vector2 cellCenterWorld = _tilemap.GetCellCenterWorld(cellPosition);
        Vector2 targetCentered = new Vector2(cellCenterWorld.x, entry.origin.y);

        DebugRayAtPosition(targetCentered, Color.red, Vector2.right);

        // Get the upper and lower bounds of the portal position
        Bounds portalBounds = _portal.GetBounds();
        Vector2 checkPosition = targetCentered - portalBounds.extents * Vector2.up;
        Vector2 topPosition = targetCentered + portalBounds.extents * Vector2.up;

        // If we shot from the right, the left tiles must be empty
        // If we shot from the left, the right tiles must be empty
        Vector3Int emptyDirection = GetVerticalEmptyDirection(entry.direction);

        DebugRayAtPosition(checkPosition, Color.red, Vector2.right);
        DebugRayAtPosition(topPosition, Color.red, Vector2.right);

        return CheckAllVerticalPositions(checkPosition, topPosition, emptyDirection);
    }

    private bool CheckAllVerticalPositions(Vector2 checkPosition, Vector2 topPosition, Vector3Int emptyDirection)
    {
        // Keep checking until we are past the top position by more than one cell size height
        bool canOpen = true;
        bool topCheck = false;
        while (checkPosition.y < topPosition.y || !topCheck)
        {
            // If we are past the top position, snap the check to the top position
            if (checkPosition.y > topPosition.y)
            {
                checkPosition.y = topPosition.y;
                topCheck = true;
            }

            // Get the cell position of our check position
            Vector3Int checkCellPosition = _tilemap.WorldToCell(checkPosition);

            // Is this a valid position to open a vertical portal?
            if (IsValidPosition(checkCellPosition, emptyDirection))
            {
                DebugRayAtPosition(checkPosition, Color.green, Vector2.right);
                checkPosition.y += _tilemap.cellSize.y;
            }
            else
            {
                canOpen = false;
                break;
            }
        }
        return canOpen;
    }

    private Vector3Int GetVerticalEmptyDirection(Vector2 entryDirection)
    {
        Vector3Int emptyDirection;
        if (entryDirection.x > 0)
        {
            emptyDirection = Vector3Int.left;
        }
        else
        {
            emptyDirection = Vector3Int.right;
        }
        return emptyDirection;
    }

    private bool IsValidPosition(Vector3Int cellPosition, Vector3Int emptyDirection)
    {
        bool hasTile = _tilemap.HasTile(cellPosition);
        bool hasNeighbor = HasNeighbor(cellPosition, emptyDirection);

        // It is a valid position if we have a tile and do not have a neighbor in the empty direction
        return hasTile && !hasNeighbor;
    }

    private bool HasNeighbor(Vector3Int cellPosition, Vector3Int neighbor)
    {
        return _tilemap.HasTile(cellPosition + neighbor) || _groundTilemap.HasTile(cellPosition + neighbor);
    }

    private void DebugRayAtPosition(Vector2 position, Color color, Vector2 direction)
    {
        Debug.DrawRay(position, direction, color, 5.0f);
    }
}
