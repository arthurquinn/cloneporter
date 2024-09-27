using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FloorSwitchActivatedEvent
{
    public string SwitchName { get; private set; }

    public FloorSwitchActivatedEvent(string switchName)
    {
        SwitchName = switchName;
    }
}

public struct FloorSwitchDeactivatedEvent
{
    public string SwitchName { get; private set; }

    public FloorSwitchDeactivatedEvent(string switchName)
    {
        SwitchName = switchName;
    }
}

public class FloorSwitchActivatedEventChannel : AbstractEventChannel<FloorSwitchActivatedEvent>
{

}

public class FloorSwitchDeactivatedEventChannel : AbstractEventChannel<FloorSwitchDeactivatedEvent>
{

}


[CreateAssetMenu(fileName = "FloorSwitchEventChannel", menuName = "EventChannels/FloorSwitchEventChannel")]
public class FloorSwitchEventChannel : ScriptableObject
{
    public FloorSwitchActivatedEventChannel OnFloorSwitchActivated {  get; private set; } = new FloorSwitchActivatedEventChannel();
    public FloorSwitchDeactivatedEventChannel OnFloorSwitchDeactivated { get; private set; } = new FloorSwitchDeactivatedEventChannel();
}
