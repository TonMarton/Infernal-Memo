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
    
        foreach (MeshCollider collider in colliders)
        {
            collider.enabled = !isOpen;
        }
        animator.SetBool("isOpen", isOpen);
       
    }
}
