using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void LaserAttack(EnemyAttack attack, Ray2D laserRay);
}

public class PlayerAttackable : MonoBehaviour, IAttackable
{
    private PlayerMovement _movement;
    private Rigidbody2D _rb;
    private HealthController _hpController;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
        _hpController = GetComponent<HealthController>();
    }

    private void OnEnable()
    {
        _hpController.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _hpController.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {

    }

    public void LaserAttack(EnemyAttack attack, Ray2D laserRay)
    {
        // Calculate the knockback direction
        Vector2 knockbackDirection = GetKnockbackDirection(laserRay);

        // Apply the knockback to our movement component
        _movement.Knockback(new KnockbackAttack(knockbackDirection, attack.KnockbackForce));

        // Take flat damage
        _hpController.TakeFlatDamage(attack.Damage);
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
