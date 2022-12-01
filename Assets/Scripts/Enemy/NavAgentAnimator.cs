using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgentAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public string moveParameterName = "Move";
    public string attackState = "Attack";

    private NavAgent navAgent;

    public void OnNavAgentAttack()
    {
        animator.Play(attackState);
    }

    private void Awake()
    {
        navAgent = GetComponent<NavAgent>();
    }

    private void Update()
    {
        animator.SetFloat(moveParameterName, navAgent.IsMoving ? 1 : 0);
    }
}
