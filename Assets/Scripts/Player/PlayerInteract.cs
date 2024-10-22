using UnityEngine;

public interface IInteractable
{
    void ShowInteractHUD();
    void HideInteractHUD();
}

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The interact trigger attached to this player game object.")]
    [SerializeField] private GenericTrigger _interactTrigger;

    private IInteractable _cachedInteractable;

    private void OnEnable()
    {
        _interactTrigger.OnTriggerEnter += HandleEnterTrigger;
        _interactTrigger.OnTriggerExit += HandleExitTrigger;
    }

    private void OnDisable()
    {
        _interactTrigger.OnTriggerEnter -= HandleEnterTrigger;
        _interactTrigger.OnTriggerExit -= HandleExitTrigger;
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
