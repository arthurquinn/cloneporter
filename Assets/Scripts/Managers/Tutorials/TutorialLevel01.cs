using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevel01 : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PanelTilesEventChannel _panelEvents;
    [SerializeField] private TutorialEventChannel _tutorialEvents;

    [Header("Trigger Areas")]
    [Tooltip("The movement info text trigger.")]
    [SerializeField] private GenericTrigger _movementInfoTrigger;
    [Tooltip("The jump info text trigger.")]
    [SerializeField] private GenericTrigger _jumpInfoTrigger;
    [Tooltip("The purple portal info text trigger")]
    [SerializeField] private GenericTrigger _purplePortalInfoTrigger;
    [Tooltip("The elevator info text trigger.")]
    [SerializeField] private GenericTrigger _elevatorInfoTrigger;

    [Header("Info Texts")]
    [Tooltip("The movement info text game object.")]
    [SerializeField] private GameObject _movementInfoText;
    [Tooltip("The jump info text game object.")]
    [SerializeField] private GameObject _jumpInfoText;
    [Tooltip("The purple portal info text game object.")]
    [SerializeField] private GameObject _purplePortalInfoText;
    [Tooltip("The teal portal info text game object.")]
    [SerializeField] private GameObject _tealPortalInfoText;
    [Tooltip("The portal info text game object.")]
    [SerializeField] private GameObject _portalInfoText;
    [Tooltip("The elevator info text game object.")]
    [SerializeField] private GameObject _elevatorInfoText;

    private bool _didShowPurple;
    private bool _didShowTeal;

    private Canvas _canvas;

    private void Awake()
    {
        AssignCamera();
    }

    private void OnEnable()
    {
        _movementInfoTrigger.OnTriggerEnter += HandleMovementInfoTriggerEnter;
        _jumpInfoTrigger.OnTriggerEnter += HandleJumpInfoTriggerEnter;
        _purplePortalInfoTrigger.OnTriggerEnter += HandlePurplePortalInfoTriggerEnter;

        _panelEvents.OnPanelPlacePortal.Subscribe(HandlePortalPlaced);

        _elevatorInfoTrigger.OnTriggerEnter += HandleElevatorInfoTriggerEnter;
    }

    private void OnDisable()
    {
        _movementInfoTrigger.OnTriggerEnter -= HandleMovementInfoTriggerEnter;
        _jumpInfoTrigger.OnTriggerEnter -= HandleJumpInfoTriggerEnter;
        _purplePortalInfoTrigger.OnTriggerEnter -= HandlePurplePortalInfoTriggerEnter;

        _panelEvents.OnPanelPlacePortal.Unsubscribe(HandlePortalPlaced);

        _elevatorInfoTrigger.OnTriggerEnter -= HandleElevatorInfoTriggerEnter;
    }

    private void Start()
    {
        _tutorialEvents.OnStateChanged.Raise(new TutorialStateChangedEvent(TutorialState.Start));
    }

    private void HandleMovementInfoTriggerEnter()
    {
        _movementInfoText.SetActive(true);
    }

    private void HandleJumpInfoTriggerEnter()
    {
        _movementInfoText.SetActive(false);
        _jumpInfoText.SetActive(true);
    }

    private void HandlePurplePortalInfoTriggerEnter()
    {
        _jumpInfoText.SetActive(false);
        _purplePortalInfoText.SetActive(true);

        _tutorialEvents.OnStateChanged.Raise(new TutorialStateChangedEvent(TutorialState.PurpleActivate));
    }

    private void HandlePortalPlaced(PanelPlacePortalEvent @event)
    {
        if (@event.Color == PortalColor.Purple && !_didShowPurple)
        {
            _purplePortalInfoText.SetActive(false);
            _tealPortalInfoText.SetActive(true);
            _didShowPurple = true;

            _tutorialEvents.OnStateChanged.Raise(new TutorialStateChangedEvent(TutorialState.TealActivate));
        }
        else if (@event.Color == PortalColor.Teal && _didShowPurple && !_didShowTeal)
        {
            _tealPortalInfoText.SetActive(false);
            _portalInfoText.SetActive(true);
            _didShowTeal = true;
        }
    }

    private void HandleElevatorInfoTriggerEnter()
    {
        _portalInfoText.SetActive(false);
        _elevatorInfoText.SetActive(true);
    }

    private void AssignCamera()
    {
        _canvas = GetComponentInChildren<Canvas>();
        if (_canvas != null)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = Camera.main;
            _canvas.sortingLayerName = "HUD";
        }
    }
}
