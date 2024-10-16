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
    PortalPlacement OpenPortal(Ray2D entry, PortalColor color);
}

public interface IPortalGround
{
    public Tilemap Tilemap { get; }
    public Tilemap Ground { get; }
    public float PortalLength { get; }
    public LayerMask PortalLayer { get; }
}

public enum OpenPortalAlgorithmType
{
    GridLocked,
    Free
}

public class PanelTiles : MonoBehaviour, IPortalGround
{
    [Header("EventChannels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private PanelTilesEventChannel _panelEventChannel;
    [SerializeField] private PortalPairEventChannel _portalEventChannel;

    [Space(20)]

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private OpenPortalAlgorithmType _openPortalAlgorithmType;
    [SerializeField] private LayerMask _portalLayer;

    private Tilemap _tilemap;

    private IOpenPortalAlgorithm _openPortalAlgorithm;

    private Vector3Int[] _activePurpleTiles;
    private Vector3Int[] _activeTealTiles;

    private readonly Vector2 OVERLAP_CHECK_DIMENSIONS = new Vector2(0.1f, 0.1f);

    private float _portalLength;

    public Tilemap Tilemap { get { return _tilemap; } }
    public Tilemap Ground { get { return _groundTilemap; } }
    public float PortalLength { get { return _portalLength; } }
    public LayerMask PortalLayer { get { return _portalLayer; } }

    private void Awake()
    {
        _activePurpleTiles = new Vector3Int[0];
        _activeTealTiles = new Vector3Int[0];

        SetOpenPortalAlgorithm();
    }

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    private void OnEnable()
    {
        _playerEventChannel.OnPortalGunFired.Subscribe(HandlePortalGunFired);
        _portalEventChannel.OnPortalPairStarted.Subscribe(HandlePortalPairStarted);
        _portalEventChannel.OnPortalPairCleared.Subscribe(HandlePortalPairCleared);
    }

    private void OnDisable()
    {
        _playerEventChannel.OnPortalGunFired.Unsubscribe(HandlePortalGunFired);
        _portalEventChannel.OnPortalPairStarted.Unsubscribe(HandlePortalPairStarted);
        _portalEventChannel.OnPortalPairCleared.Unsubscribe(HandlePortalPairCleared);
    }

    private void HandlePortalPairCleared(PortalPairClearedEvent portalClearedEvent)
    {
        ClearPortalCollision();
    }

    private void HandlePortalPairStarted(PortalPairStartedEvent portalStartedEvent)
    {
        _portalLength = portalStartedEvent.PortalLength;
    }

    private void HandlePortalGunFired(PlayerPortalGunFireEvent portalGunFiredEvent)
    {
        // If we returned a valid portal placement position, then open the portal
        PortalPlacement portalPlacement = _openPortalAlgorithm.OpenPortal(portalGunFiredEvent.AimRay, portalGunFiredEvent.PortalColor);
        if (!portalPlacement.Position.Equals(Vector2.negativeInfinity))
        {
            OpenPortalForColor(portalGunFiredEvent.PortalColor, portalPlacement);
            UpdateTileCollision(portalGunFiredEvent.PortalColor, portalPlacement);
        }
    }

    public void OpenPortalManual(PortalColor color, Ray2D manualRay)
    {
        PortalPlacement portalPlacement = _openPortalAlgorithm.OpenPortal(manualRay, color);
        if (!portalPlacement.Position.Equals(Vector2.negativeInfinity))
        {
            OpenPortalForColor(color, portalPlacement);
            UpdateTileCollision(color, portalPlacement);
        }
    }

    private void OpenPortalForColor(PortalColor color, PortalPlacement placement)
    {
        _panelEventChannel.OnPanelPlacePortal.Raise(new PanelPlacePortalEvent(
            color, placement.Position, placement.Orientation, placement.AffectedTiles));
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
        // Get the tiles that we will need to disable collision for
        Vector3Int[] collisionTiles = GetCollisionTilesForPlacement(placement);

        if (color == PortalColor.Purple)
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

        // Do not disable collision unless both portals are placed
        if (_activePurpleTiles.Length > 0 && _activeTealTiles.Length > 0)
        {
            DisableTileCollision(_activePurpleTiles);
            DisableTileCollision(_activeTealTiles);
        }
    }

    private Vector3Int[] GetCollisionTilesForPlacement(PortalPlacement placement)
    {
        // Get the tiles "behind" the tiles that will hold the portal to disable collision on them also
        Vector2 checkDirection = -placement.Orientation;
        Vector3Int[] collisionTiles = new Vector3Int[placement.AffectedTiles.Length * 2];
        for (int i = 0; i < placement.AffectedTiles.Length; i++)
        {
            Vector3Int checkTile = new Vector3Int(
                placement.AffectedTiles[i].x,
                placement.AffectedTiles[i].y);

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

    private void ClearPortalCollision()
    {
        // Reenable collision
        EnableTileCollision(_activePurpleTiles);
        EnableTileCollision(_activeTealTiles);

        // Clear the cached tiles
        _activeTealTiles = new Vector3Int[0];
        _activePurpleTiles = new Vector3Int[0];
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
                _tilemap.SetColliderType(tiles[i], Tile.ColliderType.Grid);
            }
            else if (_groundTilemap.HasTile(tiles[i]))
            {
                _groundTilemap.SetColliderType(tiles[i], Tile.ColliderType.Grid);
            }
        }
    }
}
