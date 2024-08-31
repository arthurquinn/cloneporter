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

    private SpriteRenderer _spriteRenderer;


    private bool _isPressed;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _spriteRenderer.sprite = _isPressed ? _pressedSprite : _notPressedSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isPressed)
        {
            _isPressed = true;
            _onSwitchActivated.Invoke();
        }
    }
}
