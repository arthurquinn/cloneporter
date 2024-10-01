using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ElevatorUpStartEvent
{

}

public class ElevatorUpStartEventChannel : AbstractEventChannel<ElevatorUpStartEvent>
{

}

public struct ElevatorUpStopEvent
{

}

public class ElevatorUpStopEventChannel : AbstractEventChannel<ElevatorUpStopEvent>
{

}

[CreateAssetMenu(fileName = "ElevatorEventChannel", menuName = "EventChannels/ElevatorEventChannel")]
public class ElevatorEventChannel : ScriptableObject
{
    public ElevatorUpStartEventChannel OnElevatorUp { get; private set; } = new ElevatorUpStartEventChannel();
    public ElevatorUpStopEventChannel OnElevatorStop { get; private set; } = new ElevatorUpStopEventChannel();
}
