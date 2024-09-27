using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cube : MonoBehaviour, ICarryable
{
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Drop()
    {
        _rb.isKinematic = false;

        transform.SetParent(null);
    }

    public void Pickup(Transform carryPoint)
    {
        _rb.isKinematic = true;

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
    }
}
