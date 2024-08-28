using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DupeController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void SetDupe(Vector2 position, Quaternion rotation)
    {
        _spriteRenderer.enabled = true;
        transform.position = position;
        transform.rotation = rotation;
    }

    public void ClearDupe()
    {
        _spriteRenderer.enabled = false;
    }
}
