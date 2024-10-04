using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("Scene Name Indexes")]
    [SerializeField] private SceneNames _sceneNames;

    public static GameManager Instance { get; private set; }

    private SceneNameIndex _currentScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _playerEvents.OnCompleteLevel.Subscribe(HandleLevelComplete);
    }

    private void OnDisable()
    {
        _playerEvents.OnCompleteLevel.Unsubscribe(HandleLevelComplete);
    }

    private void HandleLevelComplete(PlayerCompleteLevelEvent @event)
    {
        SceneNameIndex nextScene = _sceneNames.SceneArray[_currentScene.NextLevelIndex];
        ChangeScene(nextScene);
    }

    public void ChangeScene(SceneNameIndex scene)
    {
        _currentScene = scene;
        SceneManager.LoadScene(scene.SceneIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
