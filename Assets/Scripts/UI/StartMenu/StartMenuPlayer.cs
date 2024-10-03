using Cloneporter.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuPlayer : MonoBehaviour
{
    [SerializeField] private StartMenuEventChannel _events;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _events.OnAnimationEvent.Subscribe(HandleAnimationEvent);
    }

    private void OnDisable()
    {
        _events.OnAnimationEvent.Unsubscribe(HandleAnimationEvent);
    }

    private void HandleAnimationEvent(StartMenuAnimationEvent @event)
    {
        if (@event.EventType == StartMenuAnimationEventType.GridAnimationComplete)
        {
            StartPlayer();
        }
    }

    private void StartPlayer()
    {
        _rb.angularVelocity = 360;
        _rb.gravityScale = 0.2f;
    }
}
