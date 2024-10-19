using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialState
{
    Start,
    PurpleActivate,
    TealActivate
}

public struct TutorialStateChangedEvent
{
    public TutorialState State { get; private set; }

    public TutorialStateChangedEvent(TutorialState state)
    {
        State = state;
    }
}

public class TutorialStateChangedEventChannel : AbstractEventChannel<TutorialStateChangedEvent>
{

}

[CreateAssetMenu(fileName = "TutorialEventChannel", menuName = "EventChannels/TutorialEventChannel")]
public class TutorialEventChannel : ScriptableObject
{
    public TutorialStateChangedEventChannel OnStateChanged { get; private set; } = new TutorialStateChangedEventChannel();
}
