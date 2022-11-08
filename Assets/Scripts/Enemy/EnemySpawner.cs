using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
    // The enemy prefab to spawn with this enemy spawner
    [SerializeField] private Enemy enemyPrefab;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    // Spawn an enemy
    public Enemy SpawnEnemy()
    {
        // Small vertical offset above the spawner
        var spawnPosition = transform.position + Vector3.up * 1.0f;
        // Spawn the enemy prefab
        return Instantiate(enemyPrefab, spawnPosition, transform.rotation);
    }

    // Draw a gizmo representing the spawn point that shows a symbol for the enemy that will be spawned
    private void OnDrawGizmos()
    {
        // game playing?
        if (Application.isPlaying)
        {
            // then don't draw the gizmo. we only want to see it in the editor when not playing
            return;
        }

        // Set the color of the gizmo to green
        Gizmos.color = Color.green;

        // Draw the enemy's mesh to represent the enemy in editor
        // find the mesh filter component in children
        var meshFilter = enemyPrefab.GetComponentInChildren<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        var meshTransform = meshFilter.gameObject.transform;

        // also add half the mesh's height to the position so the gizmo is centered on the enemy
        var position = transform.position + meshTransform.localPosition + Vector3.up * (mesh.bounds.size.y) * 3;

        Gizmos.DrawMesh(mesh, 0,
            position,
            transform.rotation * meshTransform.localRotation,
            meshTransform.localScale);

        // draw an arrow for the direction facing
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}