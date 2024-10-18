using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BurnoutAttackable : MonoBehaviour, IAttackable
{
    [Header("Event Channels")]
    [SerializeField] private BurnoutEventChannel _events;

    [Header("Stats")]
    [Tooltip("The max HP for the Burnout unit.")]
    [SerializeField] private float _maxHP;
    [Tooltip("The amount of time after the most recent attack before the unit can start recovering HP.")]
    [SerializeField] private float _recoveryTime;
    [Tooltip("The amount of HP regained per second.")]
    [SerializeField] private float _recoveryAmount;

    [Header("Death Stats")]
    [Tooltip("The angular velocity applied when the unit dies. The provided value will randomly be either positive or negative when applied in the script.")]
    [SerializeField] private float _deathAngularVelocity;

    [Header("Effects")]
    [Tooltip("The shader material used when damaged.")]
    [SerializeField] private Material _damagedMaterial;
    [Tooltip("The explosion effect prefab.")]
    [SerializeField] private GameObject _explosion;

    private Rigidbody2D _rb;
    private SpriteRenderer[] _renderers;
    private MaterialPropertyBlock[] _propertyBlocks;

    private float _recoverTimeout;
    private float _currentHP;
    private float _currentHPRatio;

    // This is the max time all particle systems may live
    private const float EXPLOSION_TIME = 5f;

    // Used by other burnout components
    public UnityAction OnDeath { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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

    private void ResetHPRecoveryTime()
    {
        _recoverTimeout = _recoveryTime;
    }

    private void DisableHPRecovery()
    {
        _recoverTimeout = float.MaxValue;
    }

    private void TakeDamagePerSecond(float damage)
    {
        // Use fixed delta time since the raycast will occur every fixed update
        _currentHP -= damage * Time.fixedDeltaTime;
        if (_currentHP <= 0)
        {
            // Unit dies
            _currentHP = 0;
            Die();
        }
        else
        {
            // Reset HP recovery time
            ResetHPRecoveryTime();
        }
    }

    private void Die()
    {
        // Invoke any actions taken by other burnout components
        if (OnDeath != null)
        {
            OnDeath();
        }

        // Disable HP recovery
        DisableHPRecovery();

        // Change the rigidbody type
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Apply a small force and some angular velocity
        _rb.angularVelocity = Mathf.RoundToInt(Random.value) == 0 ? -_deathAngularVelocity : _deathAngularVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_currentHP <= 0)
        {
            // Instantiate an explosion effect at our position
            CreateExplosion();

            // Fire off burnout death event
            _events.OnDeath.Raise(new BurnoutDeathEvent(name));

            // Destroy the game object
            Destroy(gameObject);
        }
    }

    private void CreateExplosion()
    {
        Instantiate(_explosion, transform.position, Quaternion.identity);
    }

    #region Interface Methods

    public void LaserAttack(EnemyAttack attack, Ray2D laserRay)
    {
        TakeDamagePerSecond(attack.DamagePerSecond);
    }

    #endregion
}
