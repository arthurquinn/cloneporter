using DG.Tweening;
using UnityEngine;

public class DispensableCube : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void MoveCube(Vector2 target, float duration, Ease ease)
    {
        _rb.DOMove(target, duration).SetEase(ease);
    }
}
