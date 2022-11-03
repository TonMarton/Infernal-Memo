using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int startHealth = 100;

    private int health;

    // Start is called before the first frame update
    private void Awake()
    {
        health = startHealth;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}