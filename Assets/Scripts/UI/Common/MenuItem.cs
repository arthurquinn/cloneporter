using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cloneporter.UI
{
    public class MenuItem : MonoBehaviour, IPointerEnterHandler
    {
        [Header("Event Channel")]
        [SerializeField] private MenuEventChannel _menuChannel;

        [Header("Colors")]
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _baseColor;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _text;

        private int _index;

        public void SetIndex(int index)
        {
            _index = index;
        }

        public void Select()
        {
            _text.color = _selectedColor;
        }

        public void Deselect()
        {
            _text.color = _baseColor;
        }

        public void Confirm()
        {
            _menuChannel.OnItemSelected.Raise(new MenuItemSelectedEvent(_index));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _menuChannel.OnHover.Raise(new MenuItemHoverEvent(_index));
        }
    }
}

