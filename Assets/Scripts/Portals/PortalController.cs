using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PortalController : MonoBehaviour
{
    [SerializeField] private DupeController _dupe;
    [SerializeField] private UnityEvent<Vector2, Quaternion> _onPlayerStay;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 offset = collision.transform.position - transform.position;
            if (_onPlayerStay != null)
            {
                _onPlayerStay.Invoke(offset, Quaternion.identity);
            }
        }
    }

    public void SetDupe(Vector2 offset, Quaternion rotation)
    {
        Vector2 position = (Vector2)transform.position + offset;
        _dupe.SetDupe(position, rotation);
    }

    public void SetPortal(Vector2 position, Quaternion rotation)
    {
        _spriteRenderer.enabled = true;
        _boxCollider.enabled = true;
        transform.position = position;
        transform.rotation = rotation;
    }

    public void ClearPortal()
    {
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
    }
}
