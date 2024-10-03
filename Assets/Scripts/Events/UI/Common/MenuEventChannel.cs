using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MenuItemSelectedEvent
{
    public int SelectedIndex { get; private set; }

    public MenuItemSelectedEvent(int index)
    {
        SelectedIndex = index;
    }
}

public class MenuItemSelectedEventChannel : AbstractEventChannel<MenuItemSelectedEvent>
{

}

[CreateAssetMenu(fileName = "MenuEventChannel", menuName = "EventChannels/UI/MenuEventChannel")]
public class MenuEventChannel : ScriptableObject
{
    public MenuItemSelectedEventChannel OnItemSelected { get; private set; } = new MenuItemSelectedEventChannel();
}
