using Cloneporter.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuGrid : MonoBehaviour
{
    [SerializeField] private StartMenuEventChannel _events;
    [SerializeField] private PanelTiles _panels;

    [Header("Open Points")]
    [SerializeField] private Transform _purplePoint;
    [SerializeField] private Transform _tealPoint;

    private Animator _animator;
    private int _playID;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playID = Animator.StringToHash("Play");
    }

    private void OnEnable()
    {
        _events.OnAnimationEvent.Subscribe(HandleAnimationEvent);
    }

    private void OnDisable()
    {
        _events.OnAnimationEvent.Unsubscribe(HandleAnimationEvent);
    }

    private void HandleAnimationEvent(StartMenuAnimationEvent animationEvent)
    {
        if (animationEvent.EventType == StartMenuAnimationEventType.MenuAnimationComplete)
        {
            HandleMenuAnimationComplete();
        }
    }

    private void HandleMenuAnimationComplete()
    {
        _animator.SetTrigger(_playID);
    }

    private void OpenPortals()
    {
        // Get points
        Vector2 entryDirection = Vector2.down;
        Vector2 purplePoint = _purplePoint.position;
        Vector2 tealPoint = _tealPoint.position;

        // Open purple portal
        Ray2D purpleRay = new Ray2D(purplePoint, entryDirection);
        _panels.OpenPortalManual(PortalColor.Purple, purpleRay);

        // Open teal portal
        Ray2D tealRay = new Ray2D(tealPoint, entryDirection);
        _panels.OpenPortalManual(PortalColor.Teal, tealRay);

        // Raise grid complete event
        _events.OnAnimationEvent.Raise(
            new StartMenuAnimationEvent(StartMenuAnimationEventType.GridAnimationComplete));
    }

    #region Grid Animation Event Handlers

    public void HandleGridAnimationComplete()
    {
        OpenPortals();
    }

    #endregion
}
