using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 4f;
    [SerializeField] private float minMoveTime = 0.5f;
    [SerializeField] private float maxMoveTime = 2;

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
        // player out of range?
        if (!PlayerInRange())
        {
            // do nothing
            return;
        }


        // switch on the current state
        switch (state)
        {
            case State.Idle:
                // if the enemy has been idle for long enough, move to the next state
                if (idlingFor >= idleTime)
                {
                    // pick a random time to move for
                    moveTime = Random.Range(minMoveTime, maxMoveTime);
                    
                    // our next time is moving
                    state = State.Move;
                    Debug.Log("Move");
                    
                    // reset idle time
                    idlingFor = 0f;
                }
                else
                {
                    // otherwise, increment the time the enemy has been idle
                    idlingFor += Time.deltaTime;
                    
                    // stop moving
                    StopMovingTowardsPlayer();
                    
                    // face toward the player
                    FaceTowardPlayer();
                }

                break;
            case State.Move:
                // done moving?
                if (movingFor >= moveTime)
                {
                    // go to the next state
                    state = State.Attack;
                    Debug.Log("Entered Attack");
                }
                else
                {
                    // face toward the player
                    FaceTowardPlayer();
                    
                    // move toward the player
                    MoveTowardPlayer();                   
                    
                    // increment moving for
                    movingFor += Time.deltaTime;
                }

                break;
            case State.Attack:
                // ready to move to the next state?
                if (attack.IsDoneAttacking() || !ShouldAttack())
                {
                    // move to the next state
                    state = State.Idle;
                    Debug.Log("Entered Idle");
                    // set the idle time to a random value between min and max
                    idleTime = Random.Range(minIdleTime, maxIdleTime);
                    // reset moving for
                    movingFor = 0f;
                }
                // not already attacking?
                else if (!attack.IsAttacking())
                {
                    // attack the player
                    attack.Attack();
                    Debug.Log("Attacking");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    // draw debug text with current state
    private void OnGUI()
    {
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);
        var labelPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
        
        GUI.color = Color.white;
        GUI.Label(new Rect(labelPos.x, labelPos.y, 100, 100), state.ToString(), new GUIStyle(GUI.skin.label) {fontSize = 40});
        
        
        // draw an exclamation mark above the enemy's head if they have a line of sight to the player
        if (PlayerInLineOfSight())
        {
            // the label position is above the enemy's head
            labelPos.y += 100;
            // The GUI label is red
            GUI.color = Color.red;
            GUI.Label(new Rect(labelPos.x, labelPos.y - 50, 100, 100), "!", new GUIStyle(GUI.skin.label) {fontSize = 40});
            
            // draw a line from the enemy to the player
            Handles.color = Color.red;
            // the line is thick
            Handles.DrawAAPolyLine(5, transform.position, player.transform.position);
        }
    }
}