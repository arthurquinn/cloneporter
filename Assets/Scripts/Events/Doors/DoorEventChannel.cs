using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DoorOpenEvent
{
    public string Name;

    public DoorOpenEvent(string name)
    {
        Name = name;
    }
}

public class DoorOpenEventChannel : AbstractEventChannel<DoorOpenEvent>
{

}

public struct DoorClosedEvent
{
    public string Name;

    public DoorClosedEvent(string name)
    {
        Name = name;
    }
}

public class DoorClosedEventChannel : AbstractEventChannel<DoorClosedEvent>
{

}

[CreateAssetMenu(fileName = "DoorEventChannel", menuName = "EventChannels/DoorEventChannel")]
public class DoorEventChannel : ScriptableObject
{
    public DoorOpenEventChannel OnOpen { get; private set; } = new DoorOpenEventChannel();
    public DoorClosedEventChannel OnClosed { get; private set; } = new DoorClosedEventChannel();
}
