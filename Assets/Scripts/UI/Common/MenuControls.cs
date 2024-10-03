using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cloneporter.UI
{
    public class MenuControls : MonoBehaviour
    {
        [SerializeField] private MenuItem[] _menuItems;

        private PlayerInputActions _input;

        private int _currentIndex;

        private void Awake()
        {
            _input = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _input.UI.Submit.Enable();
            _input.UI.Submit.performed += HandleSubmit;

            _input.UI.Navigate.Enable();
            _input.UI.Navigate.performed += HandleNavigate;
        }

        private void OnDisable()
        {
            _input.UI.Submit.performed -= HandleSubmit;
            _input.UI.Submit.Disable();

            _input.UI.Navigate.performed -= HandleNavigate;
            _input.UI.Navigate.Disable();   
        }

        private void Start()
        {
            SetIndexes();

            // Select the current item
            SelectCurrentIndex();
        }

        private void HandleSubmit(InputAction.CallbackContext context)
        {
            ConfirmSelection();
        }

        private void HandleNavigate(InputAction.CallbackContext context)
        {
            Vector2 direction = _input.UI.Navigate.ReadValue<Vector2>();
            if (direction == Vector2.up)
            {
                ChangeCurrentIndex(1);
            }
            else if (direction == Vector2.down)
            {
                ChangeCurrentIndex(-1);
            }

            SelectCurrentIndex();
        }

        private void ConfirmSelection()
        {
            MenuItem item = _menuItems[_currentIndex];
            item.Confirm();
        }

        private void DeselectAll()
        {
            for (int i = 0; i < _menuItems.Length; i++)
            {
                MenuItem item = _menuItems[i];
                item.Deselect();
            }
        }

        private void SelectCurrentIndex()
        {
            // Deselect everything
            DeselectAll();

            // Select current index
            MenuItem currentItem = _menuItems[_currentIndex];
            currentItem.Select();
        }

        private void ChangeCurrentIndex(int value)
        {
            _currentIndex += value;
            _currentIndex %= _menuItems.Length;
            _currentIndex = Mathf.Abs(_currentIndex);
        }

        private void SetIndexes()
        {
            for (int i = 0; i < _menuItems.Length; i++)
            {
                MenuItem item = _menuItems[i];
                item.SetIndex(i);
            }
        }
    }
}