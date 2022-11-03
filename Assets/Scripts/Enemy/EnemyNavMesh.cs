using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMesh : MonoBehaviour
{
    private GameObject target;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // target the player by default
        target = GameObject.Find("Player");
        // throw error if target not found
        if (target == null)
        {
            Debug.LogError("Target not found!");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // set the agent destination to target
        agent.SetDestination(target.transform.position);
    }
}