using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorial : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private TutorialEventChannel _tutorialEvents;

    private PlayerAiming _playerAiming;

    private void Awake()
    {
        _playerAiming = GetComponent<PlayerAiming>();
    }

    private void OnEnable()
    {
        _tutorialEvents.OnStateChanged.Subscribe(HandleTutorialStateChanged);
    }

    private void OnDisable()
    {
        _tutorialEvents.OnStateChanged.Unsubscribe(HandleTutorialStateChanged);
    }

    private void HandleTutorialStateChanged(TutorialStateChangedEvent @event)
    {
        if (@event.State == TutorialState.Start)
        {
            HandleStart();
        }
        else if (@event.State == TutorialState.PurpleActivate)
        {
            HandlePurpleActivate();
        }
        else if (@event.State == TutorialState.TealActivate)
        {
            HandleTealActivate();
        }
    }

    private void HandleStart()
    {
        _playerAiming.DisableAiming();
    }

    private void HandlePurpleActivate()
    {
        _playerAiming.EnablePurpleAiming();
    }

    private void HandleTealActivate()
    {
        _playerAiming.EnableTealAiming();
    }
}
