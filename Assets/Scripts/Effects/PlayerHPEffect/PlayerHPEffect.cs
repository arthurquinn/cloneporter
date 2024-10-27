using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPEffect : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    private SpriteRenderer _renderer;
    private MaterialPropertyBlock _block;

    // The current ratio displayed by the hp effect material
    private float _currentRatio;

    // The percentage of hp that must change before an update to the effect
    //   is applied.
    private const float EFFECT_THRESHOLD = 0.01f;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _block = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        _playerEvents.OnHPChanged.Subscribe(HandlePlayerHPChanged);
    }

    private void OnDisable()
    {
        _playerEvents.OnHPChanged.Unsubscribe(HandlePlayerHPChanged);
    }

    private void HandlePlayerHPChanged(PlayerHPChangeEvent @event)
    {
        SetEffect(@event.PlayerHP.CurrentHPRatio);
    }

    private void SetEffect(float hpRatio)
    {
        // Only update the effect if the ratio changed by a significant amount
        float ratioDiff = Mathf.Abs(hpRatio - _currentRatio);
        if (ratioDiff > EFFECT_THRESHOLD)
        {
            // Calculate the effect amount
            float amount = 1 - hpRatio;

            // Round up to 1 or down to 0 if we are at the last threshold step
            if (amount > 1 - EFFECT_THRESHOLD)
            {
                amount = 1;
            }
            else if (amount < EFFECT_THRESHOLD)
            {
                amount = 0;
            }

            // Set the effect amount on the material
            _renderer.GetPropertyBlock(_block);
            _block.SetFloat("_EffectAmount", amount);
            _renderer.SetPropertyBlock(_block);

            // Update the current ratio
            _currentRatio = hpRatio;
        }
    }
}
