using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class BlockController : MonoBehaviour, ICarryable
{
    private Rigidbody2D _rb;

    private float _initialGravityScale;
    private LayerMask _initialLayerExcludeMask;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _initialGravityScale = _rb.gravityScale;
        _initialLayerExcludeMask = _rb.excludeLayers;
    }

    public void StartCarry()
    {
        _rb.gravityScale = 0;
        _rb.excludeLayers |= (1 << LayerMask.NameToLayer("Player"));
    }

    public void StopCarry()
    {
        _rb.gravityScale = _initialGravityScale;
        _rb.excludeLayers = _initialLayerExcludeMask;
    }

    public void UpdateCarryPosition(Vector2 position)
    {
        _rb.MovePosition(position);
    }
}
