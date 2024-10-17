using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;
    [SerializeField] private PauseMenuEventChannel _pauseMenuEvents;

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
        _pauseMenuEvents.OnPaused.Subscribe(HandleGamePaused);
    }

    private void OnDisable()
    {
        _playerEvents.OnCompleteLevel.Unsubscribe(HandleLevelComplete);
        _pauseMenuEvents.OnPaused.Unsubscribe(HandleGamePaused);
    }

    private void HandleLevelComplete(PlayerCompleteLevelEvent @event)
    {
        SceneNameIndex nextScene = _sceneNames.SceneArray[_currentScene.NextLevelIndex];
        ChangeScene(nextScene);
    }

    private void HandleGamePaused(PauseMenuPausedEvent @event)
    {
        Time.timeScale = @event.IsPaused ? 0 : 1;
    }

    public void ChangeScene(SceneNameIndex scene)
    {
        Debug.Log("Changing scenes from " + _currentScene.SceneName + " to " + scene.SceneName);

        _currentScene = scene;
        SceneManager.LoadScene(scene.SceneIndex);
    }

    public void ReloadLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(_sceneNames.StartMenu.SceneIndex);
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
