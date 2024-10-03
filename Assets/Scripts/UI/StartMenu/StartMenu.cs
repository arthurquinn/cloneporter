using Cloneporter.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private StartMenuEventChannel _events;
    [SerializeField] private GameObject _menuLayout;

    private Animator _animator;
    private int _playID;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playID = Animator.StringToHash("Play");
    }

    private void Start()
    {
        _menuLayout.SetActive(false);
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
        if (@event.EventType == StartMenuAnimationEventType.TitleAnimationComplete)
        {
            HandleTitleAnimationComplete();
        }
    }

    private void HandleTitleAnimationComplete()
    {
        // Enable menu next frame
        StartCoroutine(EnableMenu());

        // Start the animation
        _animator.SetTrigger(_playID);
    }

    private IEnumerator EnableMenu()
    {
        yield return null;
        _menuLayout.SetActive(true);
    }

    #region Animation Event Handlers

    public void OnAnimationPlayComplete()
    {
        _events.OnAnimationEvent.Raise(
            new StartMenuAnimationEvent(StartMenuAnimationEventType.MenuAnimationComplete));
    }

    #endregion

    #region Button Handlers

    public void OnPlayDemoClicked()
    {
        Debug.Log("Clicked");
    }

    #endregion
}
