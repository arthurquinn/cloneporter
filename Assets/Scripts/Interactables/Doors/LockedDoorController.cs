using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoorController : MonoBehaviour
{
    [Header("Materials")]
    [Tooltip("The indicator light material when locked.")]
    [SerializeField] private Material _lockedIndicatorMaterial;
    [Tooltip("The indicator light material when unlocked.")]
    [SerializeField] private Material _unlockedIndicatorMaterial;

    [Header("References")]
    [Tooltip("The top indicator light.")]
    [SerializeField] private SpriteRenderer _topIndicator;
    [Tooltip("The bottom indicator light.")]
    [SerializeField] private SpriteRenderer _bottomIndicator;

    private DoorController _doorController;

    private void Awake()
    {
        _doorController = GetComponent<DoorController>();
    }

    private void Start()
    {
        // Start with the locked material
        AssignMaterial(_lockedIndicatorMaterial);
    }

    private void OnEnable()
    {
        _doorController.OnOpen += HandleDoorOpen;
        _doorController.OnClose += HandleDoorClose;
    }

    private void OnDisable()
    {
        _doorController.OnOpen -= HandleDoorOpen;
        _doorController.OnClose -= HandleDoorClose;
    }

    private void AssignMaterial(Material material)
    {
        _topIndicator.material = material;
        _bottomIndicator.material = material;
    }

    private void HandleDoorOpen()
    {
        // Assign the open indicator materials
        AssignMaterial(_unlockedIndicatorMaterial);
    }

    private void HandleDoorClose()
    {
        Debug.LogWarning("Locked door closed. This should not occur.");
    }
}
