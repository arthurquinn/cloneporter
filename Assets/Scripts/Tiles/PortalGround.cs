using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Runtime.CompilerServices;

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

    public Tilemap Tilemap { get { return _tilemap; } }
    public Tilemap Ground { get { return _groundTilemap; } }
    public float PortalLength { get { return _portal.GetLength(); } }

    private void Awake()
    {
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
}
