using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private DeathMenu deathMenu;

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

    public void Heal(int healing)
    {
        health = Mathf.Min(healing + health, maxHealth);

        Debug.Log("Healed " + healing + " damage. Current health: " + health);
    }

    private void Die()
    {
        // TODO: play death sound with Fmod
        
        // show the death menu 
        deathMenu.Show();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}