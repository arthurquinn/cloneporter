using UnityEngine;

public class TutorialLevel01 : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PanelTilesEventChannel _panelEvents;
    [SerializeField] private TutorialEventChannel _tutorialEvents;

    [Header("Texts")]
    [SerializeField] private GameObject _movementText;
    [SerializeField] private GameObject _purplePortalText;
    [SerializeField] private GameObject _tealPortalText;
    [SerializeField] private GameObject _teleportText;

    [Header("Triggers")]
    [SerializeField] private GenericTrigger _movementShowTrigger;
    [SerializeField] private GenericTrigger _purplePortalTrigger;
    [SerializeField] private GenericTrigger _teleportHideTrigger;

    private bool _didShowPurple;
    private bool _didShowTeal;

    private void Awake()
    {
        AssignCamera();
    }

    private void OnEnable()
    {
        _movementShowTrigger.OnTriggerEnter += HandleMovementShow;
        _purplePortalTrigger.OnTriggerEnter += HandlePurplePortalShow;
        _panelEvents.OnPanelPlacePortal.Subscribe(HandlePortalPlaced);
        _teleportHideTrigger.OnTriggerEnter += HandleTeleportHide;
    }

    private void OnDisable()
    {
        _movementShowTrigger.OnTriggerEnter -= HandleMovementShow;
        _purplePortalTrigger.OnTriggerEnter -= HandlePurplePortalShow;
        _panelEvents.OnPanelPlacePortal.Unsubscribe(HandlePortalPlaced);
        _teleportHideTrigger.OnTriggerEnter -= HandleTeleportHide;
    }

    private void HandleMovementShow(Collider2D collision)
    {
        _movementText.SetActive(true);
    }

    private void HandlePurplePortalShow(Collider2D collision)
    {
        _movementText.SetActive(false);
        _purplePortalText.SetActive(true);

        _tutorialEvents.OnStateChanged.Raise(new TutorialStateChangedEvent(TutorialState.PurpleActivate));
    }
    
    private void HandleTeleportHide(Collider2D collision)
    {
        _teleportText.SetActive(false);
    }

    private void Start()
    {
        _tutorialEvents.OnStateChanged.Raise(new TutorialStateChangedEvent(TutorialState.Start));
    }

    private void HandlePortalPlaced(PanelPlacePortalEvent @event)
    {
        if (@event.Color == PortalColor.Purple && !_didShowPurple)
        {
            _purplePortalText.SetActive(false);
            _tealPortalText.SetActive(true);
            _didShowPurple = true;

            _tutorialEvents.OnStateChanged.Raise(new TutorialStateChangedEvent(TutorialState.TealActivate));
        }
        else if (@event.Color == PortalColor.Teal && _didShowPurple && !_didShowTeal)
        {
            _tealPortalText.SetActive(false);
            _teleportText.SetActive(true);
            _didShowTeal = true;
        }
    }

    private void AssignCamera()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "HUD";
        }
    }
}
