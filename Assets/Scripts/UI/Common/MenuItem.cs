using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Cloneporter.UI
{
    public class MenuItem : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private MenuEventChannel _menuChannel;

        [Header("Colors")]
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _baseColor;

        private TextMeshProUGUI _text;

        private int _index;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

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
    }
}

