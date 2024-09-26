using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISnappable
{
    void SnapOffset(Vector2 offset);
}

[RequireComponent(typeof(Collider2D))]
public class ColliderSnap : MonoBehaviour
{
    [SerializeField] private string[] _tagsToHandle;

    private Collider2D _collider;

    private readonly Vector2 OFFSET_ADJUSTMENT = new Vector2(0.025f, 0.025f);

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0;  i < _tagsToHandle.Length; i++)
        {
            if (collision.collider.CompareTag(_tagsToHandle[i]))
            { 
                HandleCollision(collision);
            }
        }
    }

    private void HandleCollision(Collision2D collision)
    {
        ISnappable snappable = collision.collider.GetComponent<ISnappable>();
        if (snappable != null)
        {
            HandleSnapPosition(snappable, collision);
        }
    }

    private void HandleSnapPosition(ISnappable snappable, Collision2D collision)
    {
        Vector2 myHighestPoint = _collider.bounds.max;
        Vector2 theirLowestPoint = collision.collider.bounds.min;

        Vector2 myCenter = _collider.bounds.center;
        Vector2 theirCenter = collision.collider.bounds.center;

        // Don't snap them if they collided above us
        if (theirLowestPoint.y < myHighestPoint.y)
        {
            float offsetY = myHighestPoint.y - theirLowestPoint.y + OFFSET_ADJUSTMENT.y;
            float offsetX = theirCenter.x > myCenter.x ? -OFFSET_ADJUSTMENT.x : OFFSET_ADJUSTMENT.x;

            snappable.SnapOffset(new Vector2(offsetX, offsetY));
        }
    }
}
