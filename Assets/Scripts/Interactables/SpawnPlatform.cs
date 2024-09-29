using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPlatform : MonoBehaviour
{
    [SerializeField] private SpawnerEventChannel _spawnerEvents;
    [SerializeField] private Transform _spawnPoint;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        //_spawnerEvents.OnSpawnPlayer.Raise(new SpawnPlayerEvent(_spawnPoint.position));
    }
}
