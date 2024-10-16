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

    public PortalPlacement OpenPortal(Ray2D entry, PortalColor color)
    {
        Vector3Int[] placementTiles;

        Vector3Int cellPosition = _portalGround.Tilemap.WorldToCell(entry.origin);
        if (_portalGround.Tilemap.HasTile(cellPosition))
        {
            // Check if we can place a portal vertically
            placementTiles = GetVerticalPlacementTiles(cellPosition, entry, color);
            if (placementTiles != null)
            {
                // Get the center cell of the placement tiles
                Vector3Int centerCell = placementTiles[placementTiles.Length / 2];

                // Get the cell centered target location for the portal
                Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(centerCell);

                // Return the portal placement for this portal
                return new PortalPlacement(cellCenterWorld, GetVerticalOrientation(entry.direction), placementTiles);
            }

            // Check if we can place a portal horizontally
            placementTiles = GetHorizontalPlacementTiles(cellPosition, entry, color);
            if (placementTiles != null)
            {
                // Get the center cell of the placement tiles
                Vector3Int centerCell = placementTiles[placementTiles.Length / 2];

                // Get the cell centered target location for the portal
                Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(centerCell);

                // Return the portal placement for this portal
                return new PortalPlacement(cellCenterWorld, GetHorizontalOrientation(entry.direction), placementTiles);
            }
        }
        return new PortalPlacement(Vector2.negativeInfinity, Vector2.negativeInfinity, null);
    }

    private Vector3Int[] GetVerticalPlacementTiles(Vector3Int cellPosition, Ray2D entry, PortalColor color)
    {
        // Get the cell centered target location for the portal
        Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(cellPosition);

        // Calculate portal height in tiles
        int portalTileHeight = Mathf.CeilToInt(_portalGround.PortalLength / _portalGround.Tilemap.cellSize.y);
        Debug.AssertFormat(portalTileHeight % 2 == 1, "Portal of length {0} cannot be centered on tiles", _portalGround.PortalLength);

        // Calculate how many tiles up and down we will need to check
        int tileExtents = (portalTileHeight - 1) / 2;
        int startTileOffset = -tileExtents;

        // Get the empty direction to check
        Vector3Int emptyDirection = GetVerticalEmptyDirection(entry.direction);

        // Iterate the tiles for potential vertical placement
        Vector3Int[] affectedTiles = IterateVerticalPlacementTiles(startTileOffset, tileExtents, cellPosition, emptyDirection, portalTileHeight, color);

        // If we couldn't find a valid spot in the center, try the extents
        // This will let us "nudge" the portal a few spaces over if there is a valid position near the edge of a panel/ground border
        if (affectedTiles == null)
        {
            for (int i = 0; i < tileExtents; i++)
            {
                // The extent offset to check
                int extentOffset = i + 1;

                // Check the up extent
                Vector3Int upExtentCell = cellPosition + (extentOffset * Vector3Int.up);
                affectedTiles = IterateVerticalPlacementTiles(startTileOffset, tileExtents, upExtentCell, emptyDirection, portalTileHeight, color);

                // If valid we can break here
                if (affectedTiles != null)
                {
                    break;
                }

                // Check the down extent
                Vector3Int downExtentCell = cellPosition + (extentOffset * Vector3Int.down);
                affectedTiles = IterateVerticalPlacementTiles(startTileOffset, tileExtents, downExtentCell, emptyDirection, portalTileHeight, color);

                // If valid we can break here
                if (affectedTiles != null)
                {
                    break;
                }
            }
        }

        // Return the affected tiles if any (can be null)
        return affectedTiles;
    }

    private Vector3Int[] IterateVerticalPlacementTiles(int startTileOffset, int tileExtents, Vector3Int cellPosition, Vector3Int emptyDirection, int portalTileHeight, PortalColor color)
    {
        // Create array to store the tiles that would be affected
        Vector3Int[] affectedTiles = new Vector3Int[portalTileHeight];

        // Check all tiles needed to hold the portal above and below the center point (where they shot)
        // For example if our tile height is 9, then start tile offset is -4 (i.e. we check 4 below, the middle, then 4 above)
        for (int tileOffset = startTileOffset; tileOffset <= tileExtents; tileOffset++)
        {
            // Check tile at our current offset
            Vector3Int checkTile = cellPosition + (tileOffset * Vector3Int.up);

            // Check if the tile at the current offset is a valid position to hold a portal
            Vector2 checkTileWorldPosition = _portalGround.Tilemap.GetCellCenterWorld(checkTile);
            if (IsValidPosition(checkTile, emptyDirection, color))
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

        // Return affected tiles
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

    private Vector3Int[] GetHorizontalPlacementTiles(Vector3Int cellPosition, Ray2D entry, PortalColor color)
    {
        // Get the cell centered target location for the portal
        Vector2 cellCenterWorld = _portalGround.Tilemap.GetCellCenterWorld(cellPosition);

        // Calculate portal length in tiles
        int portalTileHeight = Mathf.CeilToInt(_portalGround.PortalLength / _portalGround.Tilemap.cellSize.x);
        Debug.AssertFormat(portalTileHeight % 2 == 1, "Portal of height {0} cannot be centered on tiles", _portalGround.PortalLength);

        // Calculate how many tiles left and right we will need to check
        int tileExtents = (portalTileHeight - 1) / 2;
        int startTileOffset = -tileExtents;

        // Get the empty direction to check
        Vector3Int emptyDirection = GetHorizontalEmptyDirection(entry.direction);

        // Iterate through and check the placement tiles
        Vector3Int[] affectedTiles = IterateHorizontalPlacementTiles(startTileOffset, tileExtents, cellPosition, emptyDirection, portalTileHeight, color);

        // If we couldn't find a valid spot in the center, try the extents
        // This will let us "nudge" the portal a few spaces over if there is a valid position near the edge of a panel/ground border
        if (affectedTiles == null)
        {
            for (int i = 0; i < tileExtents; i++)
            {
                // The extent offset to check
                int extentOffset = i + 1;

                // Check the right extent
                Vector3Int rightExtentCell = cellPosition + (extentOffset * Vector3Int.right);
                affectedTiles = IterateHorizontalPlacementTiles(startTileOffset, tileExtents, rightExtentCell, emptyDirection, portalTileHeight, color);

                // If valid we can break here
                if (affectedTiles != null)
                {
                    break;
                }

                // Check the left extent
                Vector3Int leftExtentCell = cellPosition + (extentOffset * Vector3Int.left);
                affectedTiles = IterateHorizontalPlacementTiles(startTileOffset, tileExtents, leftExtentCell, emptyDirection, portalTileHeight, color);

                // If valid we can break here
                if (affectedTiles != null)
                {
                    break;
                }
            }
        }

        // Return the affected tiles if any (can be null)
        return affectedTiles;
    }

    private Vector3Int[] IterateHorizontalPlacementTiles(int startTileOffset, int tileExtents, Vector3Int cellPosition, Vector3Int emptyDirection, int portalTileHeight, PortalColor color)
    {
        // Create array to store the tiles that would be affected
        Vector3Int[] affectedTiles = new Vector3Int[portalTileHeight];

        // Check all tiles needed to hold the portal to the left and right of the center point (where they shot)
        // For example if our tile height is 9, then start tile offset is -4 (i.e. we check 4 left, the middle, then 4 right)
        for (int tileOffset = startTileOffset; tileOffset <= tileExtents; tileOffset++)
        {
            // Check tile at our current offset
            Vector3Int checkTile = cellPosition + (tileOffset * Vector3Int.right);

            // Check if the tile at the current offset is a valid position to hold a portal
            Vector2 checkTileWorldPosition = _portalGround.Tilemap.GetCellCenterWorld(checkTile);
            if (IsValidPosition(checkTile, emptyDirection, color))
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

        // Return the affected tiles
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

    private bool IsValidPosition(Vector3Int checkTile, Vector3Int emptyDirection, PortalColor color)
    {
        bool hasTile = _portalGround.Tilemap.HasTile(checkTile);
        bool hasNeighbor = HasNeighbor(checkTile, emptyDirection);
        bool hasPortal = HasPortal(checkTile, color);

        // It is a valid position if we:
        //   - have a tile and
        //   - do not have a neighbor in the empty direction and
        //   - are not overlapping a portal (of a different color)
        return hasTile && !hasNeighbor && !hasPortal;
    }

    private bool HasPortal(Vector3Int checkTile, PortalColor openingColor)
    {
        // Get the center of the cell
        Vector2 cellCenter = _portalGround.Tilemap.GetCellCenterWorld(checkTile);

        // Check if it is overlapping a portal
        Collider2D overlap = Physics2D.OverlapPoint(cellCenter, _portalGround.PortalLayer);
        if (overlap != null)
        {
            // Ignore if the overlapping portal is the color we are trying to place
            IPortal overlapPortal = overlap.GetComponent<IPortal>();
            if (overlapPortal != null)
            {
                // Return true if we are overlapping a portal of a different color
                // We can ignore portals of the same color since we are effectively overwriting its placement
                return overlapPortal.Color != openingColor;
            }
        }

        // Return false if we are not overlapping anything
        return false;
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
