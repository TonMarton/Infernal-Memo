using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawnTrigger : MonoBehaviour
{
    // Has the player entered the trigger yet?
    private bool playerEntered = false;
    
    // Reference to the enemy spawn volume
    [SerializeField]
    private EnemySpawnVolume enemySpawnVolume;
    
    // Reference to the player
    private GameObject player;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Error if no enemy spawn volume was assigned
        if (enemySpawnVolume == null)
        {
            Debug.LogError($"No enemy spawn volume assigned to {gameObject.name}");
        }
        
        // hold a reference to the player
        player = GameObject.Find("Player");
        
        // Error if the player was not found
        if (player == null)
        {
            Debug.LogError($"No player found in scene");
        }
    }
    
    // On trigger enter
    private void OnTriggerEnter(Collider other)
    {
        // Don't double-trigger
        if (playerEntered)
        {
            return;
        }

        // Ignore if it wasn't the player
        if (other.gameObject != player)
        {
            return;
        }
        
        Debug.Log($"Player entered trigger {gameObject.name}");

        // Set the player entered flag to true
        playerEntered = true;
                
        // Spawn enemy's by calling spawn on the enemy spawn volume
        enemySpawnVolume.SpawnEnemies();
    }
    
    // Render a gizmo of the trigger's volume
    private void OnDrawGizmos()
    {
        // Get the trigger's position
        var triggerPosition = transform.position;
        
        // Get the trigger's size
        var triggerSize = transform.localScale;
        
        // Draw the trigger's volume
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(triggerPosition, triggerSize);
    }
}
