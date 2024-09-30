using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;
    [SerializeField] private SpawnerEventChannel _spawnerEvents;

    [Tooltip("The teleport trigger is required to fire off a player teleport event from this script when the player teleports.")]
    [SerializeField] private TeleportTrigger _teleportTrigger;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _playerEvents.OnPlayerStarted.Raise(new PlayerStartedEvent(transform));
    }

    private void OnEnable()
    {
        _spawnerEvents.OnSpawnPlayer.Subscribe(HandleSpawnPlayer);
        _teleportTrigger.OnPortalLeave += HandlePortalLeave;
    }

    private void OnDisable()
    {
        _spawnerEvents.OnSpawnPlayer.Unsubscribe(HandleSpawnPlayer);
        _teleportTrigger.OnPortalLeave += HandlePortalLeave;
    }

    private void HandleSpawnPlayer(SpawnPlayerEvent spawnPlayerEvent)
    {
        _rb.isKinematic = true;
        _rb.position = spawnPlayerEvent.SpawnPosition;
        _rb.isKinematic = false;
    }

    private void HandlePortalLeave()
    {
        // Fire off a player teleported event when we leave the portal
        _playerEvents.OnTeleported.Raise(new PlayerTeleportedEvent());
    }
}
