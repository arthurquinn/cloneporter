using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAttack", menuName = "EnemyAttacks/NewEnemyAttack")]
public class EnemyAttack : ScriptableObject
{
    public float Damage;
    public Vector2 KnockbackForce;
}
