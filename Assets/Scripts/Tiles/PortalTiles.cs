using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPortalTiles
{
    void HighlightTile(Vector2 position);
}

public class PortalTiles : MonoBehaviour, IPortalTiles
{
    [SerializeField] private LayerMask _interactLayer;

    private Tilemap _tilemap;

    private float _overlapRadius;

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    public void HighlightTile(Vector2 position)
    {
        Vector3Int cellPos = _tilemap.WorldToCell(position);

        Debug.Log(cellPos);

        TileBase tile = _tilemap.GetTile(cellPos);

        Debug.Log(tile);

        _tilemap.SetTileFlags(cellPos, TileFlags.None);
        _tilemap.SetColor(cellPos, Color.red);
    }
}
