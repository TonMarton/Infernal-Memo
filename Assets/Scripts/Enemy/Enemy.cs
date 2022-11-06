using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyAI))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    public const string EnemyTag = "Enemy";
}