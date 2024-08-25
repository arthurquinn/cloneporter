using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPortalTiles
{
    bool HasTile(Vector2 atPosition);
    void PlacePortal(Vector2 atPosition, Vector2 fromDirection);
    void HighlightTile(Vector2 position);
}

public class PortalTiles : MonoBehaviour, IPortalTiles
{
    [Header("Prefabs")]
    [SerializeField] private Portal _portal;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private LayerMask _groundLayer;

    // Components
    private Tilemap _tilemap;

    private readonly Quaternion _rotateRight = Quaternion.Euler(0, 0, 90);

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    public bool HasTile(Vector2 position)
    {
        Vector3Int cell = _tilemap.WorldToCell(position);
        return _tilemap.HasTile(cell);
    }

    public void PlacePortal(Vector2 atPosition, Vector2 fromDirection)
    {
        Vector3Int cell = _tilemap.WorldToCell(atPosition);
        if (_tilemap.HasTile(cell))
        {
            // If there is a tile to the right or left we will place the portal horizontally
            if (_tilemap.HasTile(cell + Vector3Int.up) || _tilemap.HasTile(cell + Vector3Int.down))
            {
                PlacePortalVertical(cell, atPosition, fromDirection);
            }
            else if (_tilemap.HasTile(cell + Vector3Int.right) || _tilemap.HasTile(cell + Vector3Int.left))
            {
                PlacePortalHorizontal(cell, atPosition, fromDirection);
            }
        }
    }

    private void PlacePortalHorizontal(Vector3Int cell, Vector2 atPosition, Vector2 fromPosition)
    {
        // Get center of cell we hit
        Vector2 cellSize = _tilemap.cellSize;
        Vector2 cellCenter = _tilemap.GetCellCenterWorld(cell);

        // Calculate portal placement
        Vector2 portalPlacement = new Vector2(atPosition.x, cellCenter.y);
        float portalWidth = cellSize.x * 3;

        // Adjust down if there is a ground tile above us
        RaycastHit2D rightHit = Physics2D.Raycast(portalPlacement, Vector2.right, portalWidth / 2, _groundLayer);
        if (rightHit.collider != null)
        {
            float xAdjustment = portalPlacement.x - (portalWidth / 2 - rightHit.distance);
            portalPlacement = new Vector2(xAdjustment, cellCenter.y);
        }

        // Adjust up if there is a ground tile below us
        RaycastHit2D leftHit = Physics2D.Raycast(portalPlacement, Vector2.left, portalWidth / 2, _groundLayer);
        if (leftHit.collider != null)
        {
            float xAdjustment = portalPlacement.x + (portalWidth / 2 - leftHit.distance);
            portalPlacement = new Vector2(xAdjustment, cellCenter.y);
        }

        Instantiate(_portal, portalPlacement, _rotateRight);
    }

    private void PlacePortalVertical(Vector3Int cell, Vector2 atPosition, Vector2 fromPosition)
    {
        // Get center of cell we hit
        Vector2 cellSize = _tilemap.cellSize;
        Vector2 cellCenter = _tilemap.GetCellCenterWorld(cell);

        // Calculate portal placement
        Vector2 portalPlacement = new Vector2(cellCenter.x, atPosition.y);
        float portalHeight = cellSize.y * 3;

        // Adjust down if there is a ground tile above us
        RaycastHit2D upHit = Physics2D.Raycast(portalPlacement, Vector2.up, portalHeight / 2, _groundLayer);
        if (upHit.collider != null)
        {
            float yAdjustment = portalPlacement.y - (portalHeight / 2 - upHit.distance);
            portalPlacement = new Vector2(cellCenter.x, yAdjustment);
        }

        // Adjust up if there is a ground tile below us
        RaycastHit2D downHit = Physics2D.Raycast(portalPlacement, Vector2.down, portalHeight / 2, _groundLayer);
        if (downHit.collider != null)
        {
            float yAdjustment = portalPlacement.y + (portalHeight / 2 - downHit.distance);
            portalPlacement = new Vector2(cellCenter.x, yAdjustment);
        }

        Instantiate(_portal, portalPlacement, Quaternion.identity);
    }

    public void HighlightTile(Vector2 position)
    {
        Vector3Int cellPos = _tilemap.WorldToCell(position);
        _tilemap.SetTileFlags(cellPos, TileFlags.None);
        _tilemap.SetColor(cellPos, Color.red);
    }
}
