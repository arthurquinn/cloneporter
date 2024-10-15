using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDoorActivateMap", menuName = "EntityMaps/DoorActivateMap")]
public class DoorActivateMap : ScriptableObject
{
    public DoorActivateKeyValue[] ActivationMap;
}

[System.Serializable]
public struct DoorActivateKeyValue
{
    public string DoorID;
    public string[] ActivatorKeys;
}
