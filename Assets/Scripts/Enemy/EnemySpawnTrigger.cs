using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    private void Update()
    {
        // If the player has already entered the trigger, do nothing
        if (playerEntered)
        {
            return;
        }
        
        // Do nothing if player is not in trigger
        if (!IsPlayerInTrigger())
        {
            return;
        }

        // The player has entered the trigger
        playerEntered = true;
            
        // Spawn the enemies
        SpawnEnemies();
    }
    
    // Spawn enemy's by calling spawn on the enemy spawn volume
    private void SpawnEnemies()
    {
        enemySpawnVolume.SpawnEnemies();
    }
    
    // Check if the player is in the trigger
    private bool IsPlayerInTrigger()
    {
        // Debug print that the player has entered trigger
        Debug.Log($"Player entered trigger {gameObject.name}");
        
        // Get the player's position
        var playerPosition = player.transform.position;
        
        // Get the trigger's position
        var triggerPosition = transform.position;
        
        // Get the trigger's size
        var triggerSize = transform.localScale;

        // Check if the player is in the trigger
        return playerPosition.x > triggerPosition.x - triggerSize.x / 2 &&
               playerPosition.x < triggerPosition.x + triggerSize.x / 2 &&
               playerPosition.y > triggerPosition.y - triggerSize.y / 2 &&
               playerPosition.y < triggerPosition.y + triggerSize.y / 2;
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
