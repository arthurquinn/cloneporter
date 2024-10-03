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

public struct MenuItemHoverEvent
{
    public int HoverIndex { get; private set; }

    public MenuItemHoverEvent(int index)
    {
        HoverIndex = index;
    }
}

public class MenuItemHoverEventChannel : AbstractEventChannel<MenuItemHoverEvent>
{

}

[CreateAssetMenu(fileName = "MenuEventChannel", menuName = "EventChannels/UI/MenuEventChannel")]
public class MenuEventChannel : ScriptableObject
{
    public MenuItemSelectedEventChannel OnItemSelected { get; private set; } = new MenuItemSelectedEventChannel();
    public MenuItemHoverEventChannel OnHover { get; private set; } = new MenuItemHoverEventChannel();
}
