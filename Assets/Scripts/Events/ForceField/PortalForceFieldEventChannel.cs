using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PortalForceFieldEnteredEvent
{

}

public class PortalForceFieldEnteredEventChannel : AbstractEventChannel<PortalForceFieldEnteredEvent>
{

}

[CreateAssetMenu(fileName = "PortalForceFieldEventChannel", menuName = "EventChannels/PortalForceFieldEventChannel")]
public class PortalForceFieldEventChannel : ScriptableObject
{
    public PortalForceFieldEnteredEventChannel OnForceFieldEntered { get; private set; } = new PortalForceFieldEnteredEventChannel();
}
