using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[DisallowMultipleComponent]
public class EnemyAIWakeTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawnVolume enemySpawnVolume;

    // Has the player entered the trigger yet?
    private bool playerEntered = false;

    // Reference to the player
    private GameObject player;

    // Start is called before the first frame update
    private void Awake()
    {
        // Error if no enemy AI volume was assigned
        if (enemySpawnVolume == null)
        {
            Debug.LogError($"No enemy AI wake volume assigned to {gameObject.name}");
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

        enemySpawnVolume.WakeEnemies();
    }

    // Render a gizmo of the trigger's volume
    private void OnDrawGizmos()
    {
        // Get the trigger's position
        var triggerPosition = transform.position;

        // Get the trigger's size
        var triggerSize = transform.localScale;

        // Draw the trigger's volume
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(triggerPosition, triggerSize);
    }
}