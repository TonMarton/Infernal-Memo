using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Unity MonoBehaviour script useful for making enemies walk towards the player.
/// </summary>
[RequireComponent(typeof(CharacterController))] // uses a character controller for movement
public class NavAgent : MonoBehaviour
{

    #region Debugging Variables
    public bool noTarget = false;
    #endregion


    #region Inspector Variables

    [Tooltip("Rate at which the script will calculate a new movement path.")]
    [SerializeField]
    private float checkPathUpdateRate = 1.0f;

    [Tooltip("The agent will randomly change direction when the distance from the target is greater than this value.")]
    [SerializeField]
    private float zigZagDistanceThreshold = 5.0f;

    [Tooltip("The time in seconds the controller has to wait between flipping directions while its sides are being touched. (No homo)")]
    [SerializeField]
    private float flipOnSidesTriggerInterval = 0.3f;

    [Header("Attack")]
    [Tooltip("Agent will attack when less than this distance from the player.")]
    [SerializeField]
    private float attackRange = 3f;

    [Tooltip("Agent attack sphere-cast radius.")]
    [SerializeField]
    private float attackRadius = 0.1f;

    [Tooltip("Agent attack sphere-cast radius.")]
    [SerializeField]
    private LayerMask attackLayers = default;

    [Tooltip("Attack cool-down takes this long.")]
    [SerializeField]
    private float attackFrequency = 1.0f;

    [Tooltip("Enemy attack damage to victims.")]
    [SerializeField]
    private int attackDamageAmount = 10;

    [Tooltip("How fast is the enemy.")]
    [SerializeField][FormerlySerializedAs("speed")]
    private float movementSpeed = 3;

    [Tooltip("Defines how many degrees of freedom the player has to be around the enemy to wake.")]
    [SerializeField]
    private float targetSightAngleThreshold = 90;

    [Tooltip("Distance for seeing the player (when within target sight angle threshold).")]
    [SerializeField]
    private float lineOfSightRange = 10000;

    [Tooltip("Distance for seeing the player (disregards target sight angle threshold.")]
    [SerializeField]
    private float closeWakeRange = 10f;

    [Header("Layermask used for line-of-sight raycasting.")]
    [SerializeField]
    private LayerMask lineOfSightLayers = default;

    [SerializeField]
    private float gravity = 30.0f;

    [SerializeField]
    private float attackDelay;

    [Header("Sound")]
    [SerializeField[FormerlySerializedAs("attackFloatingSkullSoundEvent")]
    private FMODUnity.EventReference attackSoundEvent;

    #endregion


    #region External References (Private)

    private Transform target;
    private Collider targetCollider;
    private Collider myCollider;

    #endregion


    #region External References (Public)

    public CharacterController characterController { get; private set; }

    #endregion


    #region State (Private)

    private GameObject player;
    private PlayerStats playerStats;
    private bool agentIsAwake;
    private Vector3 moveDirection;
    private Vector3[] navCorners = new Vector3[2];
    private Vector3 navDirection = default;
    private float navCalcPathElapsedTime;
    private float navLastFlipTime;
    private NavMeshPath navPath;

    #endregion


    #region State (Public)

    public bool IsMoving
    {
        get;
        private set;
    }

    public float AgentAttackCurrentCooldownTime
    {
        get;
        private set;
    }

    #endregion


    #region Sounds (Private)

    private FMOD.Studio.EventInstance attackSoundInstance;

    #endregion


    #region Events

    public UnityEvent onAttack;

    #endregion


    #region Unity Messages
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        playerStats = player.GetComponent<PlayerStats>();
        targetCollider = target.GetComponent<Collider>();
        myCollider = GetComponent<Collider>();
        navPath = new NavMeshPath();
        navCalcPathElapsedTime = 0.0f;
        IsMoving = false;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Debug.DrawLine(myCollider.bounds.center, targetCollider.bounds.center);

        UpdatePath();
        UpdateMovement();
    }
    #endregion

    private void RandomTurnLeftRight()
    {
        // Randomly turn left and right when far away
        if (Vector3.Distance(transform.position, target.position) > zigZagDistanceThreshold)
        {
            int rand = Random.Range(0, 6); // 16% chance to turn left, 16% chance to turn right
            if (rand == 0)
            {
                navDirection = Quaternion.Euler(0, 30, 0) * navDirection;
            }
            else if (rand == 1)
            {
                navDirection = Quaternion.Euler(0, -30, 0) * navDirection;
            }
        }
    }

    private bool IsTargetInFront()
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

    protected bool IsTargetInCloseRange()
    {
        // get the range to the player
        float range = Vector3.Distance(transform.position, target.position);

        // check if player is within range
        return range <= closeWakeRange;
    }

    protected bool PlayerInLineOfSight()
    {
        // do a raycast to check whether player is in line of sight of the enemy
        Vector3 origin = myCollider.bounds.center;
        Vector3 rayDirection = targetCollider.bounds.center - origin;
        if (!Physics.Raycast(origin, rayDirection, out RaycastHit hit, lineOfSightRange, lineOfSightLayers))
        {
            // raycast didn't hit anything so player isn't in line of sight
            return false;
        }

        // did the raycast hit the player?
        return hit.collider == targetCollider;
    }

    private Vector3 GetAttackDirection()
    {
        Vector3 attackOrigin = myCollider.bounds.center;
        Vector3 attackDirection = navDirection;
        Vector3 closestPoint = targetCollider.ClosestPoint(attackOrigin);
        Vector3 playerDifference = closestPoint - attackOrigin;
        attackDirection.y = playerDifference.y;
        attackDirection.Normalize();
        return attackDirection;
    }
    private void UpdateMovement()
    {
        float moveDirectionY = moveDirection.y;
        moveDirection = (IsMoving && AgentAttackCurrentCooldownTime <= 0) ? movementSpeed * navDirection : Vector3.zero;
        moveDirection.y = moveDirectionY;

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        if (IsMoving)
        {
            // Calculate rotation from direction
            transform.rotation = Quaternion.LookRotation(navDirection);

            // Check if the controller collided on its sides
            if ((characterController.collisionFlags & CollisionFlags.Sides) != 0)
            {
                if (Time.time > navLastFlipTime + .4f)
                {
                    if (Physics.Raycast(new Vector3(myCollider.bounds.center.x, myCollider.bounds.min.y + 0.2f, myCollider.bounds.center.z), transform.forward, out RaycastHit wallHit, 5.0f, 1 << 0))
                    {
                        navDirection = Vector3.Reflect(navDirection, wallHit.normal);
                        navLastFlipTime = Time.time;
                        float sign = 1;
                        if (Random.value < 0.5f) sign *= -1;
                        navDirection = Quaternion.Euler(0, Random.Range(30, 45) * sign, 0) * navDirection;
                    }
                }
            }
        }
    }
    private void UpdatePath()
    {
        // update the way to the goal every second.
        navCalcPathElapsedTime += Time.deltaTime * checkPathUpdateRate;

        // calculate path / update direction loop
        if (navCalcPathElapsedTime > 1.0f)
        {
            navCalcPathElapsedTime -= 1.0f;

            if (PlayerInLineOfSight() && (IsTargetInCloseRange() || IsTargetInFront()))
            {
                agentIsAwake = true;
            }

            if (agentIsAwake)
            {
                if (!noTarget)
                {
                    if (playerStats.isDead)
                    {
                        int rand = Random.Range(0, 30);
                        if (rand == 0)
                        {
                            navDirection = Quaternion.Euler(0, 30, 0) * navDirection;
                        }
                        else if (rand == 1)
                        {
                            navDirection = Quaternion.Euler(0, -30, 0) * navDirection;
                        }
                    }
                    else
                    {
                        // try to calculate a path from the nav mesh
                        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, navPath))
                        {
                            // calculate desired agent direction from the first two path points
                            navDirection = (navCorners[1] - navCorners[0]);
                            navDirection.y = 0;
                            navDirection.Normalize();

                            // begin moving
                            IsMoving = true;

                            // random movement
                            RandomTurnLeftRight();
                        }
                        else
                        {
                            navDirection = target.position - transform.position;
                            navDirection.y = 0;
                            navDirection.Normalize();
                        }
                    }
                }
                else
                {
                    agentIsAwake = false;
                    IsMoving = false;
                }
            }
        }

        AttackLoop();
    }

    private void AttackLoop()
    {
        // attack loop
        if (agentIsAwake)
        {
            // reduce timer
            if (AgentAttackCurrentCooldownTime > 0)
            {
                AgentAttackCurrentCooldownTime -= Time.deltaTime * attackFrequency;
            }

            // try attacking
            if (AgentAttackCurrentCooldownTime <= 0)
            {
                Vector3 attackDirection = GetAttackDirection();

                if (Physics.SphereCast(myCollider.bounds.center,
                                       attackRadius,
                                       attackDirection,
                                       out RaycastHit attackHit,
                                       attackRange,
                                       attackLayers,
                                       QueryTriggerInteraction.Ignore))
                {
                    // check if player
                    if (attackHit.collider.CompareTag("Player"))
                    {
                        // attack sound
                        StartCoroutine(PlayAttackSound());

                        // check if not dead
                        PlayerStats playerStats = attackHit.collider.GetComponent<PlayerStats>();


                        if (!playerStats.isDead)
                        {
                            // TODO: play player hurt sound?
                            AgentAttackCurrentCooldownTime += 1f;
                            onAttack.Invoke();
                            StopAllCoroutines();
                            StartCoroutine(AttackWithDelay());
                        }

                    }
                }

                // if the cooldown timer was below zero and never attacked, reset to 0
                if (AgentAttackCurrentCooldownTime < 0)
                {
                    AgentAttackCurrentCooldownTime = 0;
                }
            }
        }
    }

    #region Coroutines
    private IEnumerator PlayAttackSound()
    {
        SoundUtils.PlaySound3D(ref attackSoundInstance, attackSoundEvent, gameObject);
        yield return new WaitForSeconds(0);
    }

    private IEnumerator AttackWithDelay()
    {
        yield return new WaitForSeconds(attackDelay);

        // attack
        Vector3 attackDirection = GetAttackDirection();
        if (Physics.SphereCast(myCollider.bounds.center, attackRadius, attackDirection, out RaycastHit attackHit, attackRange, attackLayers, QueryTriggerInteraction.Ignore))
        {
            // check if player
            if (attackHit.collider.CompareTag("Player"))
            {
                // check if not dead
                PlayerStats playerStats = attackHit.collider.GetComponent<PlayerStats>();
                if (!playerStats.isDead)
                {
                    playerStats.TakeDamage(attackDamageAmount, gameObject);
                }
            }
        }
    }
    #endregion
}