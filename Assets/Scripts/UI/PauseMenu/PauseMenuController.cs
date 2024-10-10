using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _layout;
    [SerializeField] private MenuEventChannel _menuEvents;
    [SerializeField] private PauseMenuEventChannel _pauseEvents;

    private PlayerInputActions _input;

    private bool _isPaused;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.UI.Pause.Enable();
        _input.UI.Pause.performed += HandlePausePerformed;

        _menuEvents.OnItemSelected.Subscribe(HandleItemSelected);
    }

    private void OnDisable()
    {
        _input.UI.Pause.performed -= HandlePausePerformed;
        _input.UI.Pause.Disable();

        _menuEvents.OnItemSelected.Unsubscribe(HandleItemSelected);
    }

    private void HandlePausePerformed(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
    }

    private void HandleItemSelected(MenuItemSelectedEvent @event)
    {
        if (@event.SelectedIndex == 0)
        {
            TogglePauseMenu();
        }
        else if (@event.SelectedIndex == 1)
        {
            TogglePauseMenu();
            GameManager.Instance.ReloadLevel();
        }
        else if (@event.SelectedIndex == 2)
        {
            TogglePauseMenu();
            GameManager.Instance.QuitToMainMenu();
        }
    }

    private void TogglePauseMenu()
    {
        _isPaused = !_isPaused;
        _layout.SetActive(_isPaused);
        _pauseEvents.OnPaused.Raise(new PauseMenuPausedEvent(_isPaused));
    }
}
