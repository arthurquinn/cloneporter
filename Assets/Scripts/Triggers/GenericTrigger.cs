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

    public UnityAction OnTriggerEnter;
    public UnityAction OnTriggerExit;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnTriggerEnter != null)
        {
            OnTriggerEnter();
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
            OnTriggerExit();
        }

        if (_disableOnExit)
        {
            _collider.enabled = false;
        }
    }
}
