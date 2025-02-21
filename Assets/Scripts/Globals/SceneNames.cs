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
    public SceneNameIndex Level06;
    public SceneNameIndex Level07;
    public SceneNameIndex Level08;
    public SceneNameIndex Level09;
    public SceneNameIndex Level10;
    public SceneNameIndex Level11;
    public SceneNameIndex Level12;
    public SceneNameIndex Level13;

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
            Level06,
            Level07,
            Level08,
            Level09,
            Level10,
            Level11,
            Level12,
            Level13,
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
