using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
public class DoorController : MonoBehaviour
{
    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool _isOpen;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        if (!_isOpen)
        {
            _isOpen = true;
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
        }
    }

    public void CloseDoor()
    {
        if (_isOpen)
        {
            _isOpen = false;
            _collider.enabled = true;
            _spriteRenderer.enabled = true;
        }
    }
}
