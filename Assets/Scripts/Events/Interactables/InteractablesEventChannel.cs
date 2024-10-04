using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HeldItemDroppedEvent
{
    public ICarryable Item;

    public HeldItemDroppedEvent(ICarryable item)
    {
        Item = item;
    }
}

public class HeldItemDroppedEventChannel : AbstractEventChannel<HeldItemDroppedEvent>
{

}

[CreateAssetMenu(fileName = "InteractablesEventChannel", menuName = "EventChannels/InteractablesEventChannel")]
public class InteractablesEventChannel : ScriptableObject
{
    public HeldItemDroppedEventChannel OnItemDropped {  get; private set; } = new HeldItemDroppedEventChannel();
}
