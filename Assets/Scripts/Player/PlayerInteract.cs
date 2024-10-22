using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
    void ShowInteractHUD();
    void HideInteractHUD();
}

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The interact trigger attached to this player game object.")]
    [SerializeField] private GenericTrigger _interactTrigger;

    private PlayerInputActions _input;

    private IInteractable _cachedInteractable;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Interact.Enable();
        _input.Player.Interact.performed += HandleInteract;

        _interactTrigger.OnTriggerEnter += HandleEnterTrigger;
        _interactTrigger.OnTriggerExit += HandleExitTrigger;
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= HandleInteract;
        _input.Player.Interact.Disable();

        _interactTrigger.OnTriggerEnter -= HandleEnterTrigger;
        _interactTrigger.OnTriggerExit -= HandleExitTrigger;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (_cachedInteractable != null)
        {
            _cachedInteractable.Interact();
        }
    }

    private void HandleEnterTrigger(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.ShowInteractHUD();
            _cachedInteractable = interactable;
        }
    }

    private void HandleExitTrigger(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.HideInteractHUD();
            _cachedInteractable = null;
        }
    }
}
