using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneNames", menuName = "Globals/SceneNames")]
public class SceneNames : ScriptableObject
{
    public SceneNameIndex StartMenu;
    public SceneNameIndex Level01;
}

[System.Serializable]
public struct SceneNameIndex
{
    public int SceneIndex;
    public string SceneName;
}
