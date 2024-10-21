using UnityEngine;

public class TutorialLevel05 : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("Texts")]
    [SerializeField] private GameObject _pickupText;
    [SerializeField] private GameObject _dropText;

    [Header("Triggers")]
    [SerializeField] private GenericTrigger _pickupTrigger;

    private bool _didShowDrop;

    private void Start()
    {
        AssignCamera();
    }

    private void OnEnable()
    {
        _pickupTrigger.OnTriggerEnter += HandlePickupEnter;
        _playerEvents.OnPickupItem.Subscribe(HandlePickupItem);
        _playerEvents.OnDropItem.Subscribe(HandleDropItem);
    }

    private void OnDisable()
    {
        _pickupTrigger.OnTriggerEnter -= HandlePickupEnter;
        _playerEvents.OnPickupItem.Unsubscribe(HandlePickupItem);
        _playerEvents.OnDropItem.Unsubscribe(HandleDropItem);
    }

    private void HandlePickupEnter()
    {
        _pickupText.SetActive(true);
    }

    private void HandlePickupItem(PlayerPickupItemEvent @event)
    {
        if (!_didShowDrop)
        {
            _pickupText.SetActive(false);
            _dropText.SetActive(true);
            _didShowDrop = true;
        }
    }

    private void HandleDropItem(PlayerDropItemEvent @event)
    {
        _dropText.SetActive(false);
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
