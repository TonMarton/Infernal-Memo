using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] 
    private int startingHealth = 100;

    public int health { get; private set; }
    public int armor { get; private set; }
    public int shells { get; private set; } = 0;
    
    // Start is called before the first frame update
    private void Awake()
    {
        // Initialize health 
        health = startingHealth;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        // Print how much damage was taken, and the player's current health
        Debug.Log("Took " + damage + " damage. Current health: " + health);
        
        if (health <= 0)
        {
            Die();
            return;
        }
        
        // TODO: play hurt sound with Fmod
    }
    
    private void Die()
    {
        // TODO: play death sound with Fmod
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
