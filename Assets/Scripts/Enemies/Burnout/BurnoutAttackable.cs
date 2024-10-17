using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnoutAttackable : MonoBehaviour, IAttackable
{
    [Header("Stats")]
    [Tooltip("The max HP for the Burnout unit.")]
    [SerializeField] private float _maxHP;
    [Tooltip("The amount of time after the most recent attack before the unit can start recovering HP.")]
    [SerializeField] private float _recoveryTime;
    [Tooltip("The amount of HP regained per second.")]
    [SerializeField] private float _recoveryAmount;

    private SpriteRenderer[] _renderers;

    private float _recoverTimeout;
    private float _currentHP;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _currentHP = _maxHP;
    }

    private void Update()
    {
        RecoverHP();
        SetHPColor();
    }

    private void RecoverHP()
    {
        // Decrement recover timeout
        _recoverTimeout -= Time.deltaTime;

        // Attempt to re-enable HP recovery if the timeout has expired
        if (_recoverTimeout < 0)
        {
            _currentHP += _recoveryAmount * Time.deltaTime;
            _currentHP = Mathf.Min(_currentHP, _maxHP);
        }
    }

    private void SetHPColor()
    {
        // Calculate the color we should be based on current HP
        float hpRatio = _currentHP / _maxHP;
        Color color = Color.Lerp(Color.red, Color.white, hpRatio);

        // Set the color to our sprite renderers
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].color = color;
        }
    }

    private void DisableRecovery()
    {
        _recoverTimeout = _recoveryTime;
    }

    private void TakeDamagePerSecond(float damage)
    {
        // Use fixed delta time since the raycast will occur every fixed update
        _currentHP -= damage * Time.fixedDeltaTime;
        if (_currentHP <= 0)
        {
            _currentHP = 0;
            Debug.Log("I am dead.");
        }

        // Disable HP recovery
        DisableRecovery();
    }

    #region Interface Methods

    public void LaserAttack(EnemyAttack attack, Ray2D laserRay)
    {
        TakeDamagePerSecond(attack.DamagePerSecond);
    }

    #endregion
}
