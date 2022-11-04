using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private float minIdleTime = 0.5f;
    [SerializeField] private float maxIdleTime = 1f;
    [SerializeField] private float minMoveTime = 1.5f;
    [SerializeField] private float maxMoveTime = 2f;

    // enum for the different states the enemy can be in
    private enum State
    {
        Idle,
        Move,
        Attack
    }

    private State state = State.Idle;

    private float idlingFor;
    private float idleTime;

    private void Update()
    {
        // switch on the current state
        switch (state)
        {
            case State.Idle:
                // if the enemy has been idle for long enough, move to the next state
                if (idlingFor >= idleTime)
                {
                    state = State.Move;
                    Debug.Log("Entered Move");
                    idlingFor = 0f;
                }
                else
                {
                    // otherwise, increment the time the enemy has been idle
                    idlingFor += Time.deltaTime;
                }

                break;
            case State.Move:
                // move toward the player
                MoveTowardPlayer();
                // if the enemy is close enough to attack, move to the next state
                if (ShouldAttack())
                {
                    state = State.Attack;
                    Debug.Log("Entered Attack");
                }

                break;
            case State.Attack:
                // is the player not attacking yet?
                if (!attack.IsAttacking())
                {
                    // attack the player
                    attack.Attack();
                    Debug.Log("Attacking");
                }
                // else is the player done attacking?
                else if (attack.IsDoneAttacking())
                {
                    // move to the next state
                    state = State.Idle;
                    Debug.Log("Entered Idle");
                    // set the idle time to a random value between min and max
                    idleTime = Random.Range(minIdleTime, maxIdleTime);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}