using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Player Move Stats")]
public class PlayerMoveStats : ScriptableObject
{
    [Header("Stats")]
    [Range(4f, 16f)] public float RunSpeed;
}