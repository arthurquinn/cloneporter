using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneNames", menuName = "Globals/SceneNames")]
public class SceneNames : ScriptableObject
{
    public SceneNameIndex StartMenu;
    public SceneNameIndex Level01;
    public SceneNameIndex Level02;
    public SceneNameIndex Level03;
    public SceneNameIndex Level04;
    public SceneNameIndex Level05;

    // TODO: Quick solution- fix this up
    private SceneNameIndex[] _sceneArray;
    public SceneNameIndex[] SceneArray { get { return _sceneArray; } }

    private void OnEnable()
    {
        _sceneArray = new SceneNameIndex[]
        {
            StartMenu,
            Level01,
            Level02,
            Level03,
            Level04,
            Level05,
        };
    }
}

[System.Serializable]
public struct SceneNameIndex
{
    public int SceneIndex;
    public string SceneName;
    public int NextLevelIndex;
}
