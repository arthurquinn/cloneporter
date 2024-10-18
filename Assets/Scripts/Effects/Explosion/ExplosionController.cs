using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [Header("Lifetime")]
    [Tooltip("The max amount of time before all particle systems complete. After this time, the game object will be destroyed.")]
    [SerializeField] private float _maxLifetime;

    private float _currentLifetime;

    private void Awake()
    {
        _currentLifetime = _maxLifetime;
    }

    private void Update()
    {
        _currentLifetime -= Time.deltaTime;

        CheckDestroy();
    }

    private void CheckDestroy()
    {
        if (_currentLifetime < 0)
        {
            Destroy(gameObject);
        }
    }
}
