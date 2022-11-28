using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorZone : MonoBehaviour
{
    [SerializeField]
    private ElevatorZone destinationElevatorZone;

    private GameObject player;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = gameObject.GetComponentInParent<LevelManager>();
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!destinationElevatorZone)
        {
            return;
        }

        if (other.gameObject != player)
        {
            return;
        }

        Debug.Log("Player entered the elevator");

        Quaternion rotationDiffernece = gameObject.transform.rotation * Quaternion.Inverse(destinationElevatorZone.transform.rotation);
        levelManager.ChangeToNextLevel(destinationElevatorZone.transform.position,rotationDiffernece);
    }

    // Draw a box in the editor to show where the enemy spawn volume is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
