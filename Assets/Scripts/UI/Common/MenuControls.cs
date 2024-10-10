using UnityEngine;
using UnityEngine.InputSystem;

namespace Cloneporter.UI
{
    public class MenuControls : MonoBehaviour
    {
        [SerializeField] private MenuEventChannel _menuEvents;
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

            _menuEvents.OnHover.Subscribe(HandleHoverEnter);
        }

        private void OnDisable()
        {
            _input.UI.Submit.performed -= HandleSubmit;
            _input.UI.Submit.Disable();

            _input.UI.Navigate.performed -= HandleNavigate;
            _input.UI.Navigate.Disable();

            _menuEvents.OnHover.Unsubscribe(HandleHoverEnter);
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
                ChangeCurrentIndex(-1);
            }
            else if (direction == Vector2.down)
            {
                ChangeCurrentIndex(1);
            }

            SelectCurrentIndex();
        }

        private void HandleHoverEnter(MenuItemHoverEvent @event)
        {
            _currentIndex = @event.HoverIndex;
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
            if (_currentIndex == -1)
            {
                _currentIndex = _menuItems.Length - 1;
            }
            else
            {
                _currentIndex %= _menuItems.Length;
                _currentIndex = Mathf.Abs(_currentIndex);
            }
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