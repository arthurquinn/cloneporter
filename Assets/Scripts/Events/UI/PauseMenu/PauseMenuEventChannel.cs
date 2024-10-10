using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PauseMenuPausedEvent
{
    public bool IsPaused { get; private set; }

    public PauseMenuPausedEvent(bool isPaused)
    {
        IsPaused = isPaused;
    }
}

public class PauseMenuPausedEventChannel : AbstractEventChannel<PauseMenuPausedEvent>
{

}

[CreateAssetMenu(fileName = "PauseMenuEventChannel", menuName = "EventChannels/UI/PauseMenuEventChannel")]
public class PauseMenuEventChannel : ScriptableObject
{
    public PauseMenuPausedEventChannel OnPaused { get; private set; } = new PauseMenuPausedEventChannel();
}
