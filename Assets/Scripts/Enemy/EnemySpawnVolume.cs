using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawnVolume : MonoBehaviour
{
    // Array of enemy spawners in the game
    private EnemySpawner[] enemySpawners;

    private void Awake()
    {
        // Get array of enemy spawners in the game
        enemySpawners = FindObjectsOfType<EnemySpawner>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    // Search for all enemy spawner's inside the volume boundaries within the world
    public void SpawnEnemies()
    {
        // Loop through all the enemy spawners
        foreach (var enemySpawner in enemySpawners)
        {
            // Check if the enemy spawner is inside the volume
            if (IsInsideVolume(enemySpawner.transform.position))
            {
                // Spawn the enemy
                enemySpawner.SpawnEnemy();
            }

            bool IsInsideVolume(Vector3 transformPosition)
            {
                // Check if the position is within the volume
                return transformPosition.x >= transform.position.x - transform.localScale.x / 2 &&
                       transformPosition.x <= transform.position.x + transform.localScale.x / 2 &&
                       transformPosition.y >= transform.position.y - transform.localScale.y / 2 &&
                       transformPosition.y <= transform.position.y + transform.localScale.y / 2 &&
                       transformPosition.z >= transform.position.z - transform.localScale.z / 2 &&
                       transformPosition.z <= transform.position.z + transform.localScale.z / 2;
            }
        }
    }

    // Draw a box in the editor to show where the enemy spawn volume is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}