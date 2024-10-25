using Cinemachine;
using DG.Tweening;
using UnityEngine;

public interface IAttackable
{
    void LaserAttack(EnemyAttack attack, Ray2D laserRay);
}

public class PlayerAttackable : MonoBehaviour, IAttackable
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("References")]
    [Tooltip("The game object associated with the player's cinemachine impulse source.")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [Tooltip("The damage aura material to be used when player dies.")]
    [SerializeField] private Material _damageAuraMaterial;

    [Header("Stats")]
    [Tooltip("Amount of time the player is immune after being hit.")]
    [SerializeField] private float _immunityTime;

    private PlayerMovement _movement;
    private Rigidbody2D _rb;
    private HealthController _hpController;

    private SpriteRenderer[] _renderers;
    private MaterialPropertyBlock[] _blocks;

    private float _immunityTimer;
    private bool _isDead;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
        _hpController = GetComponent<HealthController>();
        _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _blocks = new MaterialPropertyBlock[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _blocks[i] = new MaterialPropertyBlock();
        }

        AssignMaterials();
    }

    private void FixedUpdate()
    {
        _immunityTimer -= Time.fixedDeltaTime;
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
        // Set dead flag
        _isDead = true;

        // Start death effect animation
        StartDeathEffectAnimation();
    }

    private void AssignMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _damageAuraMaterial;
        }
    }

    private void StartDeathEffectAnimation()
    {
        float initialValue = 0f;
        DOTween.To(() => initialValue, SetEffectAmount, 1f, 1.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(StartDeathDissolveAnimation);
    }

    private void SetEffectAmount(float amount)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].GetPropertyBlock(_blocks[i]);
            _blocks[i].SetFloat("_ColorAmount", amount);
            _blocks[i].SetFloat("_EffectAmount", amount);
            _renderers[i].SetPropertyBlock(_blocks[i]);
        }
    }

    private void StartDeathDissolveAnimation()
    {
        float initialValue = 0f;
        DOTween.To(() => initialValue, SetDissolveAmount, 1f, 5.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(CompleteDeathAnimation);
    }

    private void SetDissolveAmount(float amount)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].GetPropertyBlock(_blocks[i]);
            _blocks[i].SetFloat("_DissolveAmount", amount);
            _renderers[i].SetPropertyBlock(_blocks[i]);
        }
    }

    private void CompleteDeathAnimation()
    {
        // Raise the completed death event
        _playerEvents.OnDeath.Raise(new PlayerDeathEvent(PlayerDeathState.Completed));
    }

    public void LaserAttack(EnemyAttack attack, Ray2D laserRay)
    {
        if (!_isDead)
        {
            // Calculate the knockback direction
            Vector2 knockbackDirection = GetKnockbackDirection(laserRay);

            // Apply the knockback to our movement component
            _movement.Knockback(new KnockbackAttack(knockbackDirection, attack.KnockbackForce));

            // Start the impulse source
            _impulseSource.GenerateImpulseWithForce(1);

            // Check damage taken
            TakeDamage(attack);
        }
    }

    private void TakeDamage(EnemyAttack attack)
    {
        // Do not take damage if we are within our immunity timer
        if (_immunityTimer < 0)
        {
            // Take the flat damage
            _hpController.TakeFlatDamage(attack.Damage);

            // Reset the immunity timer
            _immunityTimer = _immunityTime;
        }
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
