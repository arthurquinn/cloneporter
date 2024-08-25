using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPortalTiles
{
    void PlacePortal(Vector2 position);
    void HighlightTile(Vector2 position);
}

public class PortalTiles : MonoBehaviour, IPortalTiles
{
    [Header("Prefabs")]
    [SerializeField] private Portal _portal;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _interactLayer;

    private Tilemap _tilemap;

    private readonly Vector3Int[] _adjacentTilesCheck = new Vector3Int[2]
    {
        Vector3Int.up,
        Vector3Int.right,
    };

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    public void PlacePortal(Vector2 position)
    {
        // Get the position of our hit cell
        Vector3Int cellPos = _tilemap.WorldToCell(position);

        // Check adjacent tiles
        for (int i = 0; i < _adjacentTilesCheck.Length; i++)
        {
            // Set up tile checks
            // We will check two tiles out in the initial direction, and the opposite of the initial direction
            //   (i.e. if our tile check is up, we will check two up and two down)
            Vector3Int tileCheck = cellPos + _adjacentTilesCheck[i];
            Vector3Int tileCheckAdjacent = tileCheck + _adjacentTilesCheck[i];
            Vector3Int tileCheckOpposite = cellPos - _adjacentTilesCheck[i];
            Vector3Int tileCheckOppositeAdjacent = tileCheckOpposite - _adjacentTilesCheck[i];

            // Perform the checks
            bool hasTileCheck = _tilemap.HasTile(tileCheck);
            bool hasTileCheckAdjacent = _tilemap.HasTile(tileCheckAdjacent);
            bool hasTileCheckOpposite = _tilemap.HasTile(tileCheckOpposite);
            bool hasTileCheckOppositeAdjacent = _tilemap.HasTile(tileCheckOppositeAdjacent);

            // Based on the above checks, figure out where to place the tile
            // The comments will assume we are working with Vector3Int.up for simplicity
            if (hasTileCheck && hasTileCheckOpposite)
            {
                // If the tile above and below are filled, we can place the portal in the center tile
                CreatePortal(cellPos);
            }
            else if (hasTileCheck && hasTileCheckAdjacent)
            {
                // If the tile above, and the next tile above are filled, we will place the portal
                //   in the tile directly above us
                CreatePortal(tileCheck);
            }
            else if (hasTileCheckOpposite && hasTileCheckOppositeAdjacent)
            {
                // If the tile below and the next tile below are filled, we will place the portal
                //   in the tile directly below us
                CreatePortal(tileCheckOpposite);
            }
        }
    }

    public void HighlightTile(Vector2 position)
    {
        Vector3Int cellPos = _tilemap.WorldToCell(position);
        _tilemap.SetTileFlags(cellPos, TileFlags.None);
        _tilemap.SetColor(cellPos, Color.red);
    }

    private void CreatePortal(Vector3Int position)
    {
        // Get the center cell position
        Vector3 cellCenterPos = _tilemap.GetCellCenterWorld(position);

        // Instantiate the portal
        Instantiate(_portal, cellCenterPos, Quaternion.identity);
    }
}
