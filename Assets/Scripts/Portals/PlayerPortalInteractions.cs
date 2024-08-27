using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlayerPortalInteractions : MonoBehaviour
{
    private CapsuleCollider2D _collider;

    [SerializeField] private LayerMask _ignoreCollisionOnPortalEnter;

    private void Start()
    {
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Turn off collision for specified ignore layers
        LayerMask current = Physics2D.GetLayerCollisionMask(gameObject.layer);
        LayerMask newMask = ~_ignoreCollisionOnPortalEnter & current;
        Physics2D.SetLayerCollisionMask(gameObject.layer, newMask);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Turn on collision for specified ignore layers (i.e. stop ignoring them)
        LayerMask current = Physics2D.GetLayerCollisionMask(gameObject.layer);
        LayerMask newMask = _ignoreCollisionOnPortalEnter | current;
        Physics2D.SetLayerCollisionMask(gameObject.layer, newMask);
    }
}
