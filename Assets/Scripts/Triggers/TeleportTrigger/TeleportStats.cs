using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeleportStats", menuName = "Stats/TeleportStats")]
public class TeleportStats : ScriptableObject
{
    public float MinExitVelocityUp { get { return _minExitVelocityUp; } }
    public float MinExitVelocitySide { get { return _minExitVelocitySide; } }

    [Header("Exit Velocity Adjustments")]
    [Tooltip("The minimum velocity applied to a rigid body on the positive y axis.")]
    [SerializeField] private float _minExitVelocityUp;
    [Tooltip("The minimum exit velocity applied to the rigidbody on the x axis.")]
    [SerializeField] private float _minExitVelocitySide;
}
