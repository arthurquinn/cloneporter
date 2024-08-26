using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPosition(Vector2 position)
    {
        _spriteRenderer.enabled = true;
        transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        _spriteRenderer.enabled = true;
        transform.rotation = rotation;
    }
}
