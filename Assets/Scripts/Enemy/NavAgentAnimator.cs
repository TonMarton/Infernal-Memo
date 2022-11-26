using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgentAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public string moveParameterName = "Move";

    private NavAgent navAgent;

    private void Awake()
    {
        navAgent = GetComponent<NavAgent>();
    }

    private void Update()
    {
        animator.SetFloat(moveParameterName, navAgent.isMoving ? 1 : 0);
    }
}
