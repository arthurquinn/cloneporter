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
    [Header("EventChannels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private PortalEventChannel _purplePortalEventChannel;
    [SerializeField] private PortalEventChannel _tealPortalEventChannel;

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
        _purplePortalEventChannel.OnPortalStarted.Subscribe(HandlePortalStarted);
    }

    private void OnDisable()
    {
        _playerEventChannel.OnPortalGunFired.Unsubscribe(HandlePortalGunFired);
        _purplePortalEventChannel.OnPortalStarted.Unsubscribe(HandlePortalStarted);
    }

    private void HandlePortalStarted(PortalStartedEvent portalStartedEvent)
    {
        _portalLength = portalStartedEvent.PortalLength;
    }

    private void HandlePortalGunFired(PlayerPortalGunFireEvent portalGunFiredEvent)
    {
        // If we returned a valid portal placement position, then open the portal
        PortalPlacement portalPlacement = _openPortalAlgorithm.OpenPortal(portalGunFiredEvent.AimRay);
        if (!portalPlacement.Position.Equals(Vector2.negativeInfinity))
        {
            if (!IsOverlapPortal(portalPlacement, portalGunFiredEvent.PortalColor))
            {
                OpenPortalForColor(portalGunFiredEvent.PortalColor, portalPlacement);
                UpdateTileCollision(portalGunFiredEvent.PortalColor, portalPlacement);
            }
        }
    }

    private void OpenPortalForColor(PortalColor color, PortalPlacement placement)
    {
        if (color == PortalColor.Purple)
        {
            _purplePortalEventChannel.OnPortalOpened.Raise(new PortalOpenedEvent(
                placement.Position, placement.Orientation, placement.AffectedTiles));
        }
        else if (color == PortalColor.Teal)
        {
            _tealPortalEventChannel.OnPortalOpened.Raise(new PortalOpenedEvent(
                placement.Position, placement.Orientation, placement.AffectedTiles));
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

        // Disable the collision on the new tiles
        DisableTileCollision(collisionTiles);
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

    private bool IsOverlapPortal(PortalPlacement placement, PortalColor color)
    {
        if (placement.Orientation.x != 0)
        {
            return IsOverlapPortalVertical(placement, color);
        }
        else
        {
            return IsOverlapPortalHorizontal(placement, color);
        }
    }

    private bool IsOverlapPortalHorizontal(PortalPlacement placement, PortalColor color)
    {
        // Get the middle tile
        int middleIndex = placement.AffectedTiles.Length / 2;
        Vector3Int middleTile = placement.AffectedTiles[middleIndex];
        Vector2 middleTileWorld = _tilemap.GetCellCenterWorld(middleTile);

        // Calculate raycast distance
        float distance = _portalLength / 2 + _tilemap.cellSize.x / 2;

        // Check raycasts
        RaycastHit2D[] rightHit = Physics2D.RaycastAll(middleTileWorld, Vector2.right, distance, _portalLayer);
        Debug.DrawLine(middleTileWorld, middleTileWorld + distance * Vector2.right, Color.yellow, 5.0f);
        if (CheckRayPortalCollision(rightHit, color))
        {
            return true;
        }

        // Check raycasts
        RaycastHit2D[] leftHit = Physics2D.RaycastAll(middleTileWorld, Vector2.left, distance, _portalLayer);
        Debug.DrawLine(middleTileWorld, middleTileWorld + distance * Vector2.left, Color.yellow, 5.0f);
        if (CheckRayPortalCollision(leftHit, color))
        {
            return true;
        }

        // No portal found, return false
        return false;
    }

    private bool IsOverlapPortalVertical(PortalPlacement placement, PortalColor color)
    {
        // Get the middle tile
        int middleIndex = placement.AffectedTiles.Length / 2;
        Vector3Int middleTile = placement.AffectedTiles[middleIndex];
        Vector2 middleTileWorld = _tilemap.GetCellCenterWorld(middleTile);

        // Calculate raycast distance
        float distance = _portalLength / 2 + _tilemap.cellSize.y / 2;

        // Check raycasts
        RaycastHit2D[] upHit = Physics2D.RaycastAll(middleTileWorld, Vector2.up, distance, _portalLayer);
        Debug.DrawLine(middleTileWorld, middleTileWorld + distance * Vector2.up, Color.yellow, 5.0f);
        if (CheckRayPortalCollision(upHit, color))
        {
            return true;
        }


        RaycastHit2D[] downHit = Physics2D.RaycastAll(middleTileWorld, Vector2.down, distance, _portalLayer);
        Debug.DrawLine(middleTileWorld, middleTileWorld + distance * Vector2.down, Color.yellow, 5.0f);
        if (CheckRayPortalCollision(downHit, color))
        {
            return true;
        }

        // No portal found, return false
        return false;
    }

    private bool CheckRayPortalCollision(RaycastHit2D[] hits, PortalColor sameColor)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                // Check if it's the same color, if so we are fine
                IPortal portal = hits[i].collider.GetComponent<IPortal>();
                if (portal != null && portal.Color != sameColor)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
