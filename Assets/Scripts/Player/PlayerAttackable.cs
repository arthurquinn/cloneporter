using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void Attack(EnemyAttack attack);
}

public class PlayerAttackable : MonoBehaviour, IAttackable
{
    [Header("Player Stats")]
    [SerializeField] private float _maxHP;

    [Header("Laser Attack")]
    [Tooltip("How much the laser will slow down player movement.")]
    [SerializeField] private float _laserSlowdown;

    private PlayerMovement _movement;
    private Rigidbody2D _rb;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Attack(EnemyAttack attack)
    {
        //_movement.CurrentAttack = attack;
    }
}
