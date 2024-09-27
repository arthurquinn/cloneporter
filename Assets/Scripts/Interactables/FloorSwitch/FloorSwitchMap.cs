using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloorSwitchMap", menuName = "EntityMaps/FloorSwitchMap")]
public class FloorSwitchMap : ScriptableObject
{
    // TODO: Try and figure out how to serialize a dictionary to optimize this a bit
    [Tooltip("Map for what objects should respond to which floor switch events. Currently, duplicate switch IDs are not allowed.")]
    public List<FloorSwitchKeyValue> Map;
}

[System.Serializable]
public class FloorSwitchKeyValue
{
    public string SwitchID;
    public string EntityID;
}
