using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;
    [SerializeField] private SpawnerEventChannel _spawnerEvents;
    [SerializeField] private ElevatorEventChannel _elevatorEvents;

    [Tooltip("The teleport trigger is required to fire off a player teleport event from this script when the player teleports.")]
    [SerializeField] private TeleportTrigger _teleportTrigger;

    [Header("Layer Masks")]
    [Tooltip("Controls what layers are ignored while an elevator is in motion.")]
    [SerializeField] private LayerMask _elevatorIgnoreLayer;

    [Header("Transforms")]
    [Tooltip("Used by the camera to determine lookahead when aiming.")]
    [SerializeField] private Transform _cameraPoint;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _playerEvents.OnPlayerStarted.Raise(new PlayerStartedEvent(transform, _cameraPoint));
    }

    private void OnEnable()
    {
        _spawnerEvents.OnSpawnPlayer.Subscribe(HandleSpawnPlayer);
        _elevatorEvents.OnElevatorUp.Subscribe(HandleElevatorUp);
        _elevatorEvents.OnElevatorStop.Subscribe(HandleElevatorStop);
        _teleportTrigger.OnTeleported += HandleTeleported;
    }

    private void OnDisable()
    {
        _spawnerEvents.OnSpawnPlayer.Unsubscribe(HandleSpawnPlayer);
        _elevatorEvents.OnElevatorUp.Unsubscribe(HandleElevatorUp);
        _elevatorEvents.OnElevatorStop.Unsubscribe(HandleElevatorStop);
        _teleportTrigger.OnTeleported -= HandleTeleported;
    }

    private void HandleSpawnPlayer(SpawnPlayerEvent spawnPlayerEvent)
    {
        _rb.isKinematic = true;
        _rb.position = spawnPlayerEvent.SpawnPosition;
        _rb.isKinematic = false;
    }

    private void HandleElevatorUp(ElevatorUpStartEvent elevatorUpEvent)
    {
        // Disable teleport trigger
        _teleportTrigger.gameObject.SetActive(false);

        // Update collision matrix
        RemoveFromCollisionMatrix(_elevatorIgnoreLayer);
    }

    private void HandleElevatorStop(ElevatorUpStopEvent elevatorStopEvent)
    {
        // Enable teleport trigger
        _teleportTrigger.gameObject.SetActive(true);

        // Update collision matrix
        AddToCollisionMatrix(_elevatorIgnoreLayer);

        // Fire off the level complete event
        _playerEvents.OnCompleteLevel.Raise(new PlayerCompleteLevelEvent());
    }

    private void HandleTeleported()
    {
        // Fire off a player teleported event when we leave the portal
        _playerEvents.OnTeleported.Raise(new PlayerTeleportedEvent());
    }

    private void RemoveFromCollisionMatrix(LayerMask mask)
    {
        // Removes collision with the layers specified in mask
        LayerMask currentMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
        currentMask &= ~mask;
        Physics2D.SetLayerCollisionMask(gameObject.layer, currentMask);
    }

    private void AddToCollisionMatrix(LayerMask mask)
    {
        // Enables collision with the layers specified in mask
        LayerMask currentMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
        currentMask |= mask;
        Physics2D.SetLayerCollisionMask(gameObject.layer, currentMask);
    }
}
