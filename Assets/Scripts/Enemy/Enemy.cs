using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStats))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    public const string EnemyTag = "Enemy";
    public const string EnemyLayer = "Enemy";
}