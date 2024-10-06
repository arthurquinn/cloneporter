using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void Attack(EnemyAttack attack, Vector2 origin);
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

    public void Attack(EnemyAttack attack, Vector2 origin)
    {
        // Calculate the knockback direction
        Vector2 knockbackDirection = GetKnockbackDirection(origin);

        // Apply the knockback to our movement component
        _movement.Knockback(new KnockbackAttack(knockbackDirection, attack.KnockbackForce));
    }

    private Vector2 GetKnockbackDirection(Vector2 origin)
    {
        if (_rb.position.x > origin.x)
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.left;
        }
    }
}
