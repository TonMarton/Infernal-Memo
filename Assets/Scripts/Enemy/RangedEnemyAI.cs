using System;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 4f;
    [SerializeField] private float minMoveTime = 0.5f;
    [SerializeField] private float maxMoveTime = 2;
    [SerializeField] private float moveDistance = 0.5f;
    private Vector3? movePoint;

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
    private float movingFor;
    private float moveTime;

    private void Update()
    {
        if (!isAwake) {
            return;
        }

        PerformActionForCurrentState();

        // Draw a thick arrow to the player
        Debug.DrawLine(transform.position, player.transform.position, Color.red, Time.deltaTime, false);

        void UpdateDebugText(string extra, bool inRange = true)
        {
            var textMeshPro = GetComponentInChildren<TMPro.TextMeshPro>();
            var outputString = $"{state} {extra}";
            if (!PlayerInRange())
            {
                outputString += " (...)";
            }

            textMeshPro.text = outputString;
        }

        void PerformActionForCurrentState()
        {
            // switch on the current state
            switch (state)
            {
                case State.Idle:
                    UpdateDebugText(idleTime.ToString());

                    // if the enemy has been idle for long enough, move to the next state
                    if (idlingFor >= idleTime)
                    {
                        // pick a random time to move for
                        moveTime = Random.Range(minMoveTime, maxMoveTime);

                        // our next time is moving
                        state = State.Move;

                        // reset the move point
                        movePoint = null;

                        // reset idle time
                        idlingFor = 0f;
                    }
                    else
                    {
                        // otherwise, increment the time the enemy has been idle
                        idlingFor += Time.deltaTime;

                        // stop moving
                        StopMoving();
                    }

                    break;
                case State.Move:
                    UpdateDebugText(moveTime.ToString());

                    // done moving?
                    if (movingFor >= moveTime)
                    {
                        // go to the next state
                        state = State.Attack;
                    }
                    else
                    {
                        // Is the player within range?
                        if (PlayerInRange())
                        {
                            // face toward the player
                            FaceTowardPlayer();

                            // move toward the player
                            MoveTowardPlayer();
                        }
                        else
                        {
                            // move point not yet chosen?
                            if (movePoint == null)
                            {
                                // choose a facing dir 
                                var facingDir = Random.insideUnitCircle.normalized;

                                // Look toward that direction
                                transform.LookAt(transform.position + new Vector3(facingDir.x, 0, facingDir.y));

                                // Pick a point in that direction to move toward
                                movePoint = transform.position + transform.forward * moveDistance;
                            }

                            // move to the point
                            MoveTowardPoint(movePoint.Value);
                        }

                        // increment moving for
                        movingFor += Time.deltaTime;
                    }

                    break;
                case State.Attack:
                    UpdateDebugText("");

                    var moveToNextState = false;

                    // not attacking yet?
                    if (attack.attackState == AttackState.NotAttacking)
                    {
                        // should we attack?
                        if (ShouldAttack())
                        {
                            // stop moving
                            StopMoving();
                            
                            // face toward player
                            FaceTowardPlayer();

                            // attack the player
                            attack.Attack();
                        }
                        // shouldn't attack?
                        else
                        {
                            // then move to the next state
                            moveToNextState = true;
                        }
                    }
                    // done attacking?
                    else if (attack.attackState == AttackState.AttackFinished)
                    {
                        // then let's move to the next state
                        moveToNextState = true;
                    }

                    // move to next state?
                    if (moveToNextState)
                    {
                        // reset attack state
                        attack.NoLongerAttacking();

                        // move to the next state
                        state = State.Idle;
                        // set the idle time to a random value between min and max
                        idleTime = Random.Range(minIdleTime, maxIdleTime);
                        // reset moving for
                        movingFor = 0f;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    // draw debug text with current state
    private void OnGUI()
    {
    }
}