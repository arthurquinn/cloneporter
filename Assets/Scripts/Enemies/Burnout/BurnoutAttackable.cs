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

    [Header("Effects")]
    [Tooltip("The shader material used when damaged.")]
    [SerializeField] private Material _damagedMaterial;

    private SpriteRenderer[] _renderers;
    private MaterialPropertyBlock[] _propertyBlocks;

    private float _recoverTimeout;
    private float _currentHP;
    private float _currentHPRatio;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _currentHP = _maxHP;
    }

    private void Start()
    {
        SetupRenderers();
    }

    private void Update()
    {
        RecoverHP();
        SetHPColor();
    }

    private void SetupRenderers()
    {
        // Create material property block array
        _propertyBlocks = new MaterialPropertyBlock[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            // Set the damaged material to all sprite renderers
            _renderers[i].material = _damagedMaterial;

            // Create a material property block for each renderer
            _propertyBlocks[i] = new MaterialPropertyBlock();
        }
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
        // Calc current hp ratio
        float hpRatio = 1 - (_currentHP / _maxHP);

        // Only update properties if our ratio changed by a significant amount
        float ratioDiff = Mathf.Abs(_currentHPRatio - hpRatio);
        if (ratioDiff > 0.01f)
        {
            Debug.Log("Changing for ratio: " + hpRatio);

            // Set proprties on each renderer based on current hp ratio
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].GetPropertyBlock(_propertyBlocks[i]);
                _propertyBlocks[i].SetFloat("_ColorAmount", hpRatio);
                _propertyBlocks[i].SetFloat("_EffectAmount", hpRatio);
                _renderers[i].SetPropertyBlock(_propertyBlocks[i]);
            }

            // Set the current ratio to avoid uneccesary updates
            _currentHPRatio = hpRatio;
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
