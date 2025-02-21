using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloneporter.UI
{
    public enum StartMenuAnimationEventType
    {
        TitleAnimationComplete,
        MenuAnimationComplete,
        GridAnimationComplete
    }

    public struct StartMenuAnimationEvent
    {
        public StartMenuAnimationEventType EventType;

        public StartMenuAnimationEvent(StartMenuAnimationEventType eventType)
        {
            EventType = eventType;
        }
    }

    public class StartMenuAnimationEventChannel : AbstractEventChannel<StartMenuAnimationEvent>
    {

    }

    public struct StartMenuReadyEvent
    {

    }

    public class StartMenuReadyEventChannel : AbstractEventChannel<StartMenuReadyEvent>
    {

    }

    [CreateAssetMenu(fileName = "StartMenuEventChannel", menuName = "EventChannels/UI/StartMenuEventChannel")]
    public class StartMenuEventChannel : ScriptableObject
    {
        public StartMenuAnimationEventChannel OnAnimationEvent { get; private set; } = new StartMenuAnimationEventChannel();
        public StartMenuReadyEventChannel OnReady { get; private set; } = new StartMenuReadyEventChannel();
    }
}