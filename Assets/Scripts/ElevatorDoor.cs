using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    private bool isOpen = false;
    private MeshCollider[] colliders;
    private Animator animator;

    void Awake()
    {
        colliders = gameObject.GetComponentsInChildren<MeshCollider>();
        animator = gameObject.GetComponent<Animator>();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        // I don't get this bug... it has null object reference otherwise

        if (colliders == null) {
            Debug.Log("Colliders were not found");
            colliders = gameObject.GetComponentsInChildren<MeshCollider>();
        } 
        foreach (MeshCollider collider in colliders)
        {
            collider.enabled = !isOpen;
        }
        if (animator == null) {
            Debug.Log("Animator were not found");
            animator = gameObject.GetComponent<Animator>();
        }
        animator.SetBool("isOpen", isOpen);
       
    }
}
