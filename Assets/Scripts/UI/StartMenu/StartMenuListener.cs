using Cloneporter.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuListener : MonoBehaviour
{
    [SerializeField] private SceneNames _scenes;
    [SerializeField] private MenuEventChannel _menuEvents;

    private void OnEnable()
    {
        _menuEvents.OnItemSelected.Subscribe(HandleItemSelected);
    }

    private void OnDisable()
    {
        _menuEvents.OnItemSelected.Unsubscribe(HandleItemSelected);
    }

    private void HandleItemSelected(MenuItemSelectedEvent @event)
    {
        if (@event.SelectedIndex == 0)
        {
            GameManager.Instance.ChangeScene(_scenes.Level01);
        }
        else if (@event.SelectedIndex == 1)
        {
            GameManager.Instance.QuitGame();
        }
    }
}
