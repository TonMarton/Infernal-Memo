using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorZone : MonoBehaviour
{
    [SerializeField]
    private ElevatorZone destinationElevatorZone;

    [SerializeField]
    private GameObject player;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = gameObject.GetComponentInParent<LevelManager>();
        Debug.Log(levelManager == null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!destinationElevatorZone) { return;  }
        // Ignore if it wasn't the player
        if (other.gameObject != player || !levelManager.allowMovingLevels)
        {
            return;
        }
        
        Debug.Log("Player entered the elevator");

        levelManager.ActivateNextLevel();

        Quaternion rotationDiffernece = gameObject.transform.rotation * Quaternion.Inverse(destinationElevatorZone.transform.rotation);

        player.GetComponent<Controller>().TeleportToPositionMaintainingRelativePosition(destinationElevatorZone.transform.position, rotationDiffernece);

        levelManager.DeactivatePreviousLevel();
    }

    // Draw a box in the editor to show where the enemy spawn volume is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
