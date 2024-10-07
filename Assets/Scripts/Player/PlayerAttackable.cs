using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void LaserAttack(EnemyAttack attack, Ray2D laserRay);
}

public class PlayerAttackable : MonoBehaviour, IAttackable
{
    [Header("Player Stats")]
    [SerializeField] private float _maxHP;

    private PlayerMovement _movement;
    private Rigidbody2D _rb;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void LaserAttack(EnemyAttack attack, Ray2D laserRay)
    {
        // Calculate the knockback direction
        Vector2 knockbackDirection = GetKnockbackDirection(laserRay);

        // Apply the knockback to our movement component
        _movement.Knockback(new KnockbackAttack(knockbackDirection, attack.KnockbackForce));
    }

    private Vector2 GetKnockbackDirection(Ray2D laserRay)
    {
        bool isVertical = laserRay.direction.y != 0;
        if (isVertical)
        {
            if (transform.position.x > laserRay.origin.x)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
        else
        {
            if (transform.position.y > laserRay.origin.y)
            {
                return Vector2.up;
            }
            else
            {
                return Vector2.down;
            }
        }
    }
}
