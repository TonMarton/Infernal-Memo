using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

// Written by Caleb Ralph
// TO-DO: Organize & clean up code more

/// <summary>
/// Unity MonoBehaviour script useful for making enemies walk towards the player.
/// </summary>
[RequireComponent(typeof(CharacterController))] // uses a character controller for movement
public class NavAgent : MonoBehaviour
{
    #region Referenced components
    private Transform target;
    public CharacterController characterController { get; private set; }

    #endregion

    #region State
    private Vector3[] corners = new Vector3[2];
    private Vector3 direction = default;
    private float elapsed;
    public bool isMoving { get; private set; }

    #endregion

    #region Parameters
    public float speed = 3;
    public float targetSightAngleThreshold = 90;
    #endregion


    #region Flags
    public bool noTarget;
    #endregion



    /// <summary>
    /// Rate at which the script will calculate a new movement path.
    /// </summary>
    [Tooltip("Rate at which the script will calculate a new movement path.")]
    public float checkPathUpdateRate = 1.0f;

    /// <summary>
    /// The agent will randomly change direction when the distance from the target is greater than this value.
    /// </summary>
    [Tooltip("The agent will randomly change direction when the distance from the target is greater than this value.")]
    public float zigZagDistanceThreshold = 5.0f;

    /// <summary>
    /// The time in seconds the controller has to wait between flipping directions while its sides are being touched. (No homo)
    /// </summary>
    [Tooltip("The time in seconds the controller has to wait between flipping directions while its sides are being touched. (No homo)")]
    public float flipOnSidesTriggerInterval = 0.3f;

    private float lastFlipTime;

    private NavMeshPath path;
    private bool isAwake;

    #region Custom Methods
    bool IsTargetInFront()
    {
        // Get direction from player to origin (sprite actor position)
        Vector3 direction = target.position - transform.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360; // ensure value is between 0 and 360

        // Take into account the actor's rotation on the Y axis (yaw)
        angle -= transform.eulerAngles.y;
        if (angle < 0) angle += 360; // ensure value is between 0 and 360

        return angle < targetSightAngleThreshold || angle > (360 - targetSightAngleThreshold);
    }

    void UpdatePath()
    {
        // Update the way to the goal every second.
        elapsed += Time.deltaTime * checkPathUpdateRate;

        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;

            if (IsTargetInFront())
            {
                isAwake = true;
            }

            if (isAwake)
            {
                if (!noTarget)
                {
                    if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
                    {
                        int cornerCount = path.GetCornersNonAlloc(corners);
                        direction = (corners[1] - corners[0]);
                        direction.y = 0;
                        direction.Normalize();
                        isMoving = true;

                        // Randomly turn left and right when far away
                        if (Vector3.Distance(transform.position, target.position) > zigZagDistanceThreshold)
                        {
                            var rand = Random.Range(0, 6);
                            if (rand == 0)
                            {
                                direction = Quaternion.Euler(0, 30, 0) * direction;

                            }
                            else if (rand == 1)
                            {
                                direction = Quaternion.Euler(0, -30, 0) * direction;
                            }
                        }
                    }
                    else
                    {
                        isMoving = false;
                    }
                }
                else
                {
                    if (noTarget) isAwake = false;
                    isMoving = false;
                }
            }
        }
    }

    void UpdateMovement()
    {
        if (isMoving)
        {
            // Calculate rotation from direction
            transform.rotation = Quaternion.LookRotation(direction);
            // Set up final movement vector
            Vector3 movement = speed * Time.deltaTime * direction;
            // Move the controller
            characterController.Move(movement);
            // Check if the controller collided on its sides
            if ((characterController.collisionFlags & CollisionFlags.Sides) != 0)
            {
                if (Time.time > lastFlipTime + 0.3f)
                {
                    lastFlipTime = Time.time;
                    float sign = 1;
                    if (Random.value < 0.5f) sign *= -1;
                    direction = Quaternion.Euler(0, Random.Range(30, 45) * sign, 0) * direction;
                }
            }
        }
    }
    #endregion



    #region Unity Messages
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        path = new NavMeshPath();
        elapsed = 0.0f;
        isMoving = false;
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        UpdatePath();
        UpdateMovement();
    }
    #endregion








}