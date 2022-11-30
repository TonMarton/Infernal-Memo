using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    private bool isOpen = false;

    private MeshCollider[] colliders;

    private Animator animator;

    void Start()
    {
        colliders = gameObject.GetComponentsInChildren<MeshCollider>();
        animator = gameObject.GetComponent<Animator>();
    }

    public void ToggleDoor()
    {
        string status = isOpen ? "closed!" : "opened!";
        Debug.Log("Elevator doors are are being  " + status);
        isOpen = !isOpen;
        foreach (MeshCollider collider in colliders)
        {
            collider.enabled = !isOpen;
        }
        animator.SetBool("isOpen", isOpen);
    }
}
