using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPortalGridLocked : IOpenPortalAlgorithm
{
    private IPortalGround _portalGround;

    public OpenPortalGridLocked(IPortalGround portalGround)
    {
        _portalGround = portalGround;
    }

    public PortalPlacement OpenPortal(Ray2D entry)
    {
        Vector3Int[] placementTiles;

        Vector3Int cellPosition = _portalGround.Tilemap.WorldToCell(entry.origin);
        if (_portalGround.Tilemap.HasTile(cellPosition))
        {
            // Check if we can place a portal vertically
            placementTiles = GetVerticalPlacementTiles(cellPosition, entry);
            if (placementTiles != null)
            {
                // Get the cell centered target location for the portal
                Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(cellPosition);

                // Return the portal placement for this portal
                return new PortalPlacement(cellCenterWorld, GetVerticalOrientation(entry.direction), placementTiles);
            }

            // Check if we can place a portal horizontally
            placementTiles = GetHorizontalPlacementTiles(cellPosition, entry);
            if (placementTiles != null)
            {
                // Get the cell centered target location for the portal
                Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(cellPosition);

                // Return the portal placement for this portal
                return new PortalPlacement(cellCenterWorld, GetHorizontalOrientation(entry.direction), placementTiles);
            }
        }
        return new PortalPlacement(Vector2.negativeInfinity, Vector2.negativeInfinity, null);
    }

    private Vector3Int[] GetVerticalPlacementTiles(Vector3Int cellPosition, Ray2D entry)
    {
        // Get the cell centered target location for the portal
        Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(cellPosition);

        // Calculate portal height in tiles
        int portalTileHeight = Mathf.CeilToInt(_portalGround.PortalLength / _portalGround.Tilemap.cellSize.y);
        Debug.AssertFormat(portalTileHeight % 2 == 1, "Portal of length {0} cannot be centered on tiles", _portalGround.PortalLength);

        // Create array to store the tiles that would be affected
        Vector3Int[] affectedTiles = new Vector3Int[portalTileHeight];

        // Calculate how many tiles up and down we will need to check
        int tileExtents = (portalTileHeight - 1) / 2;
        int startTileOffset = -tileExtents;

        // Get the empty direction to check
        Vector3Int emptyDirection = GetVerticalEmptyDirection(entry.direction);
        
        // Check all tiles needed to hold the portal above and below the center point (where they shot)
        // For example if our tile height is 9, then start tile offset is -4 (i.e. we check 4 below, the middle, then 4 above)
        for (int tileOffset = startTileOffset; tileOffset <= tileExtents; tileOffset++)
        {
            // Check tile at our current offset
            Vector3Int checkTile = cellPosition + (tileOffset * Vector3Int.up);

            // Check if the tile at the current offset is a valid position to hold a portal
            Vector2 checkTileWorldPosition = _portalGround.Tilemap.GetCellCenterWorld(checkTile);
            if (IsValidPosition(checkTile, emptyDirection))
            {
                // Store the checked tile in our affected tiles array
                affectedTiles[tileOffset + tileExtents] = checkTile;
                DebugRayAtPosition(checkTileWorldPosition, Color.green, Vector2.right);
            }
            else
            {
                // Return null if we found an invalid position
                DebugRayAtPosition(checkTileWorldPosition, Color.red, Vector2.right);
                return null;
            }
        }

        // We checked all positions and all were valid, return placement tiles
        return affectedTiles;
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

    private Vector3Int GetVerticalEmptyDirection(Vector2 entryDirection)
    {
        if (entryDirection.x > 0)
        {
            return Vector3Int.left;
        }
        else
        {
            return Vector3Int.right;
        }
    }

    private Vector3Int[] GetHorizontalPlacementTiles(Vector3Int cellPosition, Ray2D entry)
    {
        // Get the cell centered target location for the portal
        Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(cellPosition);

        // Calculate portal length in tiles
        int portalTileHeight = Mathf.CeilToInt(_portalGround.PortalLength / _portalGround.Tilemap.cellSize.x);
        Debug.AssertFormat(portalTileHeight % 2 == 1, "Portal of height {0} cannot be centered on tiles", _portalGround.PortalLength);

        // Calculate how many tiles left and right we will need to check
        int tileExtents = (portalTileHeight - 1) / 2;
        int startTileOffset = -tileExtents;

        // Create array to store the tiles that would be affected
        Vector3Int[] affectedTiles = new Vector3Int[portalTileHeight];

        // Get the empty direction to check
        Vector3Int emptyDirection = GetHorizontalEmptyDirection(entry.direction);

        // Check all tiles needed to hold the portal to the left and right of the center point (where they shot)
        // For example if our tile height is 9, then start tile offset is -4 (i.e. we check 4 left, the middle, then 4 right)
        for (int tileOffset = startTileOffset; tileOffset <= tileExtents; tileOffset++)
        {
            // Check tile at our current offset
            Vector3Int checkTile = cellPosition + (tileOffset * Vector3Int.right);

            // Check if the tile at the current offset is a valid position to hold a portal
            Vector2 checkTileWorldPosition = _portalGround.Tilemap.GetCellCenterWorld(checkTile);
            if (IsValidPosition(checkTile, emptyDirection))
            {
                // Store the checked tile in our affected tiles array
                affectedTiles[tileOffset + tileExtents] = checkTile;
                DebugRayAtPosition(checkTileWorldPosition, Color.green, Vector2.up);
            }
            else
            {
                // Return null if we found an invalid position
                DebugRayAtPosition(checkTileWorldPosition, Color.red, Vector2.up);
                return null;
            }
        }

        // We checked all positions and all were valid, return the tiles affected
        return affectedTiles;
    }

    private Vector3Int GetHorizontalEmptyDirection(Vector2 entryDirection)
    {
        if (entryDirection.y > 0)
        {
            return Vector3Int.down;
        }
        else
        {
            return Vector3Int.up;
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

    private bool IsValidPosition(Vector3Int checkTile, Vector3Int emptyDirection)
    {
        bool hasTile = _portalGround.Tilemap.HasTile(checkTile);
        bool hasNeighbor = HasNeighbor(checkTile, emptyDirection);

        // It is a valid position if we have a tile and do not have a neighbor in the empty direction
        return hasTile && !hasNeighbor;
    }

    private bool HasNeighbor(Vector3Int cellPosition, Vector3Int neighbor)
    {
        return _portalGround.Tilemap.HasTile(cellPosition + neighbor) || _portalGround.Ground.HasTile(cellPosition + neighbor);
    }

    private void DebugRayAtPosition(Vector2 position, Color color, Vector2 direction)
    {
        Debug.DrawRay(position, direction, color, 5.0f);
    }
}
