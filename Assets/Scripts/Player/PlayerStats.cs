using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] 
    private int startingHealth = 100;

    private int health;
    private int shells = 0;
    private int bullets = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize health 
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
