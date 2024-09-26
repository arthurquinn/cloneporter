using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnPlayerEvent
{
    public Vector2 SpawnPosition { get; private set; }

    public SpawnPlayerEvent(Vector2 spawnPosition)
    {
        SpawnPosition = spawnPosition;
    }
}

public class SpawnPlayerEventChannel : AbstractEventChannel<SpawnPlayerEvent>
{

}

[CreateAssetMenu(fileName = "SpawnerEventChannel", menuName = "EventChannels/SpawnerEventChannel")]
public class SpawnerEventChannel : ScriptableObject
{
    public SpawnPlayerEventChannel OnSpawnPlayer {  get; private set; } = new SpawnPlayerEventChannel();
}
