using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private Sprite _notPressedSprite;

    [SerializeField] private UnityEvent _onSwitchActivated;
    [SerializeField] private UnityEvent _onSwitchDeactivated;

    [SerializeField] private UnityEvent _onSnapCollider;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private bool _isPressed;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        _spriteRenderer.sprite = _isPressed ? _pressedSprite : _notPressedSprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _onSnapCollider?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isPressed = true;
        _onSwitchActivated?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isPressed = false;
        _onSwitchDeactivated?.Invoke();
    }
}
