using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPairController : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PortalPairEventChannel _portalEvents;
    [SerializeField] private PortalForceFieldEventChannel _forceFieldEvents;
    [SerializeField] private PanelTilesEventChannel _panelTilesEvents;

    [Header("Portal Controllers")]
    [SerializeField] private PortalController _purplePortal;
    [SerializeField] private PortalController _tealPortal;

    private void Start()
    {
        _purplePortal.SetLinkedPortal(_tealPortal);
        _tealPortal.SetLinkedPortal(_purplePortal);

        RaisePortalStartedEvent();
    }

    private void OnEnable()
    {
        _forceFieldEvents.OnForceFieldEntered.Subscribe(HandleForceFieldEntered);
        _panelTilesEvents.OnPanelPlacePortal.Subscribe(HandlePanelPlacedPortal);

        _purplePortal.OnActiveChanged += HandleActiveChanged;
        _tealPortal.OnActiveChanged += HandleActiveChanged;
    }

    private void OnDisable()
    {
        _forceFieldEvents.OnForceFieldEntered.Unsubscribe(HandleForceFieldEntered);
        _panelTilesEvents.OnPanelPlacePortal.Unsubscribe(HandlePanelPlacedPortal);

        _purplePortal.OnActiveChanged -= HandleActiveChanged;
        _tealPortal.OnActiveChanged -= HandleActiveChanged;
    }

    private void HandleActiveChanged(bool active)
    {
        _portalEvents.OnPortalActiveState.Raise(new PortalActiveStateEvent(
            active ? 
            PortalActiveState.Active : 
            PortalActiveState.Inactive));
    }

    private void HandleForceFieldEntered(PortalForceFieldEnteredEvent @event)
    {
        // Clear the portals
        _purplePortal.ClearPortal();
        _tealPortal.ClearPortal();

        // Raise the event
        _portalEvents.OnPortalPairCleared.Raise(new PortalPairClearedEvent());
    }

    private void HandlePanelPlacedPortal(PanelPlacePortalEvent @event)
    {
        if (@event.Color == PortalColor.Purple)
        {
            _purplePortal.SetPortal(@event.Position, @event.Orientation);
        }
        else if (@event.Color == PortalColor.Teal)
        {
            _tealPortal.SetPortal(@event.Position, @event.Orientation);
        }
        else
        {
            Debug.LogWarning("Unreachable code: Attempted to place portal of invalid color");
        }
    }

    private void RaisePortalStartedEvent()
    {
        // This assumes that purple and teal portals are the same size (likely won't change)
        _purplePortal.CachePortalLength();
        _tealPortal.CachePortalLength();

        // Raise the portal started event
        _portalEvents.OnPortalPairStarted.Raise(new PortalPairStartedEvent(_purplePortal.GetLength()));
    }
}
