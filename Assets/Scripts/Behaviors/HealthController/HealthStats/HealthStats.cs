using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthStats", menuName = "Stats/HealthStats")]
public class HealthStats : ScriptableObject
{
    [Header("Stats")]

    [Tooltip("The max HP for the unit.")]
    public float MaxHP;
    
    [Tooltip("The time after taking damage before hp recovery begins.")]
    public float RecoveryTimeout;

    [Tooltip("The amount of HP recovered per second while in recovery state.")]
    public float RecoveryHPS;
}
