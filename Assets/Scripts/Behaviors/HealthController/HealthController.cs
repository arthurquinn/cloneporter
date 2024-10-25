using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("The unit health stats object for this controller.")]
    [SerializeField] private HealthStats _stats;

    private float _recoverTime;
    private float _currentHP;

    public float CurrentHP { get { return _currentHP; } }
    public float CurrentHPRatio { get { return _currentHP / _stats.MaxHP; } }
    public UnityAction OnDeath { get; set; }

    private void Awake()
    {
        // Start at max hp
        _currentHP = _stats.MaxHP;
    }

    private void Update()
    {
        // Handle hp recovery
        RecoverHP();
    }

    private void RecoverHP()
    {
        // Decrement recovery timer
        _recoverTime -= Time.deltaTime;

        // Recover hp if the timeout has expired
        if (_recoverTime < 0)
        {
            _currentHP += _stats.RecoveryHPS * Time.deltaTime;
            _currentHP = Mathf.Clamp(_currentHP, 0, _stats.MaxHP);
        }
    }

    private void ResetHPRecoveryTime()
    {
        _recoverTime = _stats.RecoveryTimeout;
    }

    private void DisableHPRecoveryTime()
    {
        _recoverTime = float.MaxValue;
    }

    // Useful for attacks that occur on the fixed update loop
    public void TakeDamagePerSecondFixed(float damage)
    {
        // Update damage taken based on fixed delta time
        _currentHP -= damage * Time.fixedDeltaTime;
        if (_currentHP <= 0)
        {
            // Unit dies
            Die();
        }
        else
        {
            // Reset our recovery timer so we can start recovering HP
            //   after this attack (unless we are attacked again)
            ResetHPRecoveryTime();
        }
    }

    private void Die()
    {
        // Set hp to 0
        _currentHP = 0;

        // Disable hp recovery
        DisableHPRecoveryTime();

        // Trigger death action
        if (OnDeath != null)
        {
            OnDeath();
        }
        else
        {
            Debug.LogWarning("No OnDeath listeners for health controller on game object: " + gameObject.name);
        }
    }
}
