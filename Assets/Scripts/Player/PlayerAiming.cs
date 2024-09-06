using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAiming : MonoBehaviour
{
    private PlayerInputActions _inputs;

    private void Awake()
    {
        _inputs = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputs.Player.FireLeft.Enable();
    }

    private void OnDisable()
    {
        _inputs.Player.FireLeft.Disable();
    }

    private void Start()
    {
        _inputs.Player.FireLeft.started += AimLeftStart;
        _inputs.Player.FireLeft.canceled += AimLeftEnd;
    }

    private void AimLeftStart(InputAction.CallbackContext context)
    {

    }

    private void AimLeftEnd(InputAction.CallbackContext context)
    {

    }
}
