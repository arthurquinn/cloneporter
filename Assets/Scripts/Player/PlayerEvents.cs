using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    [SerializeField] SpawnerEventChannel _spawnerEvents;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _spawnerEvents.OnSpawnPlayer.Subscribe(HandleSpawnPlayer);
    }

    private void OnDisable()
    {
        _spawnerEvents.OnSpawnPlayer.Unsubscribe(HandleSpawnPlayer);
    }

    private void HandleSpawnPlayer(SpawnPlayerEvent spawnPlayerEvent)
    {
        _rb.isKinematic = true;
        _rb.position = spawnPlayerEvent.SpawnPosition;
        _rb.isKinematic = false;
    }
}
