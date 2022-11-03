using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // The enemy prefab to spawn with this enemy spawner
    [SerializeField]
    private Enemy enemyPrefab;
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
    // Spawn an enemy
    public void SpawnEnemy()
    {
        // Spawn the enemy prefab
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
    
    // Draw a gizmo representing the spawn point that shows a symbol for the enemy that will be spawned
    private void OnDrawGizmos()
    {
        // Set the color of the gizmo to green
        Gizmos.color = Color.green;
        
        // Draw the enemy's mesh to represent the enemy in editor
        var mesh = enemyPrefab.GetComponent<MeshFilter>().sharedMesh; 
        Gizmos.DrawMesh(mesh, 0, transform.position, transform.rotation, transform.localScale);
    }
}
