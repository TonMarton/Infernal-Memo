using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // The enemy prefab to spawn with this enemy spawner
    [SerializeField]
    private GameObject enemyPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Spawn an enemy
    public void SpawnEnemy()
    {
        // Instantiate the enemy prefab
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}
