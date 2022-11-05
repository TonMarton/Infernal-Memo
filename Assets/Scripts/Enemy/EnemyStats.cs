using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int startHealth = 100;
    [SerializeField] private float health;

    // Start is called before the first frame update
    private void Awake()
    {
        health = startHealth;
    }

    // take damage
    public void TakeDamage(float damage)
    {
        // take damage
        health -= damage;
        
        Debug.Log("Enemy took " + damage + " damage. Health is now " + health);

        // should die?
        if (health <= 0)
        {
            // die
            Die();
        }
        else
        {
            // TOD: play take damage sound with fmod
        }
    }

    // die
    private void Die()
    {
        // TODO: play death sound with fmod
        
        Debug.Log("Enemy died");

        // disable enemy
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}