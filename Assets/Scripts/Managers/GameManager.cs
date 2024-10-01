using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    public static GameManager Instance { get; private set; }

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
        Debug.Log("LEVEL COMPLETE!");
    }
}
