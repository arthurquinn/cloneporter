using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.Utilities;

public struct PortalPlacement
{
    public Vector2 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector2 Orientation { get; set; }
}

public class PortalTiles : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent<PortalPlacement> _onPurplePortalPlaced;
    [SerializeField] private UnityEvent<PortalPlacement> _onTealPortalPlaced;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _groundLayer;

    // Components
    private Tilemap _tilemap;

    private readonly Quaternion _rotateRight = Quaternion.Euler(0, 0, 90);

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    public void OnPurplePortalFired(Vector2 position, Vector2 from)
    {
        TryPlacePortal(_onPurplePortalPlaced, position, from);
    }

    public void OnTealPortalFired(Vector2 position, Vector2 from)
    {
        TryPlacePortal(_onTealPortalPlaced, position, from);
    }

    private void TryPlacePortal(UnityEvent<PortalPlacement> onPlacement, Vector2 position, Vector2 from)
    {
        // Check if we can place a portal at the adjusted hit point
        if (CanPlacePortal(position))
        {
            // Get the position and rotation that are valid according to the portal tiles tilemap
            PortalPlacement placement = GetPortalPlacement(position, from);

            // Call the event
            if (onPlacement != null)
            {
                onPlacement.Invoke(placement);
            }
        }
    }

    private bool CanPlacePortal(Vector2 position)
    {
        Vector3Int cell = _tilemap.WorldToCell(position);
        return _tilemap.HasTile(cell);
    }

    public PortalPlacement GetPortalPlacement(Vector2 position, Vector2 from)
    {
        // Get the cell at position
        Vector3Int cell = _tilemap.WorldToCell(position);

        // If there is a tile above or below we will place the portal vertically
        if (_tilemap.HasTile(cell + Vector3Int.up) || _tilemap.HasTile(cell + Vector3Int.down))
        {
            return GetVerticalPlacement(cell, position, from);
        }

        // Else if there is a tile to the right or left we will place it horizontally
        else if (_tilemap.HasTile(cell + Vector3Int.right) || _tilemap.HasTile(cell + Vector3Int.left))
        {
            return GetHorizontalPlacement(cell, position, from);
        }

        // This should never happen if we first called CanPlacePortal
        else
        {
            return new PortalPlacement();
        }
    }

    private PortalPlacement GetHorizontalPlacement(Vector3Int cell, Vector2 position, Vector2 from)
    {
        // Get center of cell we hit
        Vector2 cellSize = _tilemap.cellSize;
        Vector2 cellCenter = _tilemap.GetCellCenterWorld(cell);

        // Calculate portal placement
        Vector2 portalOrientation;
        Vector2 portalPosition = new Vector2(position.x, cellCenter.y);
        float portalWidth = cellSize.x * 3;

        // Adjust down if there is a ground tile above us
        RaycastHit2D rightHit = Physics2D.Raycast(portalPosition, Vector2.right, portalWidth / 2, _groundLayer);
        if (rightHit.collider != null)
        {
            float xAdjustment = portalPosition.x - (portalWidth / 2 - rightHit.distance);
            portalPosition = new Vector2(xAdjustment, cellCenter.y);
        }

        // Adjust up if there is a ground tile below us
        RaycastHit2D leftHit = Physics2D.Raycast(portalPosition, Vector2.left, portalWidth / 2, _groundLayer);
        if (leftHit.collider != null)
        {
            float xAdjustment = portalPosition.x + (portalWidth / 2 - leftHit.distance);
            portalPosition = new Vector2(xAdjustment, cellCenter.y);
        }

        // Are we shooting from the top? Then increment the placement, else decrement it
        if (from.y > portalPosition.y)
        {
            portalPosition.y += cellSize.y / 2;
            portalOrientation = Vector2.up;
        }
        else
        {
            portalPosition.y -= cellSize.y / 2;
            portalOrientation = Vector2.down;
        }

        return new PortalPlacement()
        {
            Position = portalPosition,
            Rotation = _rotateRight,
            Orientation = portalOrientation,
        };
    }

    private PortalPlacement GetVerticalPlacement(Vector3Int cell, Vector2 position, Vector2 from)
    {
        // Get center of cell we hit
        Vector2 cellSize = _tilemap.cellSize;
        Vector2 cellCenter = _tilemap.GetCellCenterWorld(cell);

        // Calculate portal placement
        Vector2 portalOrientation;
        Vector2 portalPosition = new Vector2(cellCenter.x, position.y);
        float portalHeight = cellSize.y * 3;

        // Adjust down if there is a ground tile above us
        RaycastHit2D upHit = Physics2D.Raycast(portalPosition, Vector2.up, portalHeight / 2, _groundLayer);
        if (upHit.collider != null)
        {
            float yAdjustment = portalPosition.y - (portalHeight / 2 - upHit.distance);
            portalPosition = new Vector2(cellCenter.x, yAdjustment);
        }

        // Adjust up if there is a ground tile below us
        RaycastHit2D downHit = Physics2D.Raycast(portalPosition, Vector2.down, portalHeight / 2, _groundLayer);
        if (downHit.collider != null)
        {
            float yAdjustment = portalPosition.y + (portalHeight / 2 - downHit.distance);
            portalPosition = new Vector2(cellCenter.x, yAdjustment);
        }

        // Are we shooting from the right? Then increment the placement, else decrement it
        if (from.x > portalPosition.x)
        {
            portalPosition.x += cellSize.x / 2;
            portalOrientation = Vector2.right;
        }
        else
        {
            portalPosition.x -= cellSize.x / 2;
            portalOrientation = Vector2.left;
        }

        return new PortalPlacement()
        {
            Position = portalPosition,
            Rotation = Quaternion.identity,
            Orientation = portalOrientation,
        };
    }
}
