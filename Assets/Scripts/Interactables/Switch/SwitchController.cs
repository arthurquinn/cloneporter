using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwitchID
{
    DoorSwitch0
}

public class SwitchController : MonoBehaviour
{
    [field: SerializeField] public SwitchID ID { get; private set; }

    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private Sprite _notPressedSprite;

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
        if (collision.CompareTag("Player"))
        {
            _isPressed = true;
        }
    }
}
