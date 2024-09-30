using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalForceField : MonoBehaviour
{
    [SerializeField] private PortalForceFieldEventChannel _forceFieldEvents;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePlayerEnter(collision);
        }
    }

    private void HandlePlayerEnter(Collider2D collision)
    {
        _forceFieldEvents.OnForceFieldEntered.Raise(new PortalForceFieldEnteredEvent());
    }
}
