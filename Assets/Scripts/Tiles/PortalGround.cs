using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public struct PortalPlacement
{
    public Vector2 Position { get; private set; }
    public Vector2 Orientation { get; private set; }
    public Vector3Int[] AffectedTiles { get; private set; }

    public PortalPlacement(Vector2 position, Vector2 orientation, Vector3Int[] affectedTiles)
    {
        Position = position;
        Orientation = orientation;
        AffectedTiles = affectedTiles;
    }
}

public interface IOpenPortalAlgorithm
{
    PortalPlacement OpenPortal(Ray2D entry);
}

public interface IPortalGround
{
    public Tilemap Tilemap { get; }
    public Tilemap Ground { get; }
    public float PortalLength { get; }
}

public enum OpenPortalAlgorithmType
{
    GridLocked,
    Free
}

public class PortalGround : MonoBehaviour, IPortalGround
{
    [SerializeField] private Tilemap _groundTilemap;

    [SerializeField] private UnityEvent<PortalPlacement> _onPurplePortalOpened;
    [SerializeField] private UnityEvent<PortalPlacement> _onTealPortalOpened;

    [SerializeField] private OpenPortalAlgorithmType _openPortalAlgorithmType;

    private PortalController _portal;
    private Tilemap _tilemap;

    private IOpenPortalAlgorithm _openPortalAlgorithm;

    private Vector3Int[] _activePurpleTiles;
    private Vector3Int[] _activeTealTiles;

    public Tilemap Tilemap { get { return _tilemap; } }
    public Tilemap Ground { get { return _groundTilemap; } }
    public float PortalLength { get { return _portal.GetLength(); } }

    private void Awake()
    {
        _activePurpleTiles = new Vector3Int[0];
        _activeTealTiles = new Vector3Int[0];

        SetOpenPortalAlgorithm();
    }

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _portal = GetComponentInChildren<PortalController>();
    }

    public void OpenPortal(PortalColor color, Ray2D entry)
    {
        // If we returned a valid portal placement position, then open the portal
        PortalPlacement portalPlacement = _openPortalAlgorithm.OpenPortal(entry);
        if (!portalPlacement.Position.Equals(Vector2.negativeInfinity))
        {
            UpdateTileCollision(color, portalPlacement);
            OpenPortalForColor(color, portalPlacement);
        }
    }

    private void OpenPortalForColor(PortalColor color, PortalPlacement placement)
    {
        if (color == PortalColor.Purple)
        {
            _onPurplePortalOpened.Invoke(placement);
        }
        else if (color == PortalColor.Teal)
        {
            _onTealPortalOpened.Invoke(placement);
        }
        else
        {
            Debug.LogWarning("Unreachable code: Should have a portal color of teal or purple");
        }
    }

    private void SetOpenPortalAlgorithm()
    {
        if (_openPortalAlgorithmType == OpenPortalAlgorithmType.Free)
        {
            _openPortalAlgorithm = new OpenPortalFree(this);
        }
        else if (_openPortalAlgorithmType == OpenPortalAlgorithmType.GridLocked)
        {
            _openPortalAlgorithm = new OpenPortalGridLocked(this);
        }
    }

    private void UpdateTileCollision(PortalColor color, PortalPlacement placement)
    {
        // Disable collision for the new tiles
        Vector3Int[] collisionTiles = GetCollisionTilesForPlacement(placement);
        DisableTileCollision(collisionTiles);
        if (color ==  PortalColor.Purple)
        {
            // Enable tile collision for the previous tiles
            EnableTileCollision(_activePurpleTiles);

            // Update the new active purple tiles
            _activePurpleTiles = collisionTiles;
        }
        else if (color == PortalColor.Teal)
        {
            // Enable tile collision for the previous tiles
            EnableTileCollision(_activeTealTiles);

            // Update the new active purple tiles
            _activeTealTiles = collisionTiles;
        }
    }

    private Vector3Int[] GetCollisionTilesForPlacement(PortalPlacement placement)
    {
        // Get the tiles "behind" the tiles that will hold the portal to disable collision on them also
        Vector2 checkDirection = -placement.Orientation;
        Vector3Int[] collisionTiles = new Vector3Int[placement.AffectedTiles.Length * 2];
        for (int i = 0; i < placement.AffectedTiles.Length; i++)
        {
            Vector3Int checkTile = placement.AffectedTiles[i];
            Vector3Int behindTile = new Vector3Int(
                checkTile.x + (int)checkDirection.x,
                checkTile.y + (int)checkDirection.y,
                0);

            // Add the check tile and behind tile to our collision tile array
            collisionTiles[i * 2] = checkTile;
            collisionTiles[i * 2 + 1] = behindTile;
        }
        return collisionTiles;
    }

    private void DisableTileCollision(Vector3Int[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (_tilemap.HasTile(tiles[i]))
            {
                _tilemap.SetColliderType(tiles[i], Tile.ColliderType.None);
            }
            else if (_groundTilemap.HasTile(tiles[i]))
            {
                _groundTilemap.SetColliderType(tiles[i], Tile.ColliderType.None);
            }
        }
    }

    private void EnableTileCollision(Vector3Int[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (_tilemap.HasTile(tiles[i]))
            {
                _tilemap.SetColliderType(tiles[i], Tile.ColliderType.Sprite);
            }
            else if (_groundTilemap.HasTile(tiles[i]))
            {
                _groundTilemap.SetColliderType(tiles[i], Tile.ColliderType.Sprite);
            }
        }
    }
}
