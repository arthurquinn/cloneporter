using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GenericTrigger : MonoBehaviour
{
    [Header("Controls")]
    [Tooltip("If checked the trigger will deactivate after the first enter.")]
    [SerializeField] private bool _disableOnEntry;
    [Tooltip("If checked the trigger will deactivate after the first exit.")]
    [SerializeField] private bool _disableOnExit;

    public UnityAction<Collider2D> OnTriggerEnter;
    public UnityAction<Collider2D> OnTriggerExit;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnTriggerEnter != null)
        {
            OnTriggerEnter(collision);
        }

        if (_disableOnEntry)
        {
            _collider.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnTriggerExit != null)
        {
            OnTriggerExit(collision);
        }

        if (_disableOnExit)
        {
            _collider.enabled = false;
        }
    }
}
