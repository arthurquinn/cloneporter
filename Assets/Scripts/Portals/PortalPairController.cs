using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPairController : MonoBehaviour
{
    [SerializeField] private PortalForceFieldEventChannel _forceFieldEvents;

    [SerializeField] private PortalController _purplePortal;
    [SerializeField] private PortalController _tealPortal;

    private void Start()
    {
        _purplePortal.SetLinkedPortal(_tealPortal);
        _tealPortal.SetLinkedPortal(_purplePortal);
    }

    private void OnEnable()
    {
        _forceFieldEvents.OnForceFieldEntered.Subscribe(HandleForceFieldEntered);
    }

    private void OnDisable()
    {
        _forceFieldEvents.OnForceFieldEntered.Unsubscribe(HandleForceFieldEntered);
    }

    private void HandleForceFieldEntered(PortalForceFieldEnteredEvent @event)
    {
        _purplePortal.ClearPortal();
        _tealPortal.ClearPortal();
    }
}
