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
    /// <summary>
    /// Rate at which the script will calculate a new movement path.
    /// </summary>
    [Tooltip("Rate at which the script will calculate a new movement path.")]
    [SerializeField] private float checkPathUpdateRate = 1.0f;

    /// <summary>
    /// The agent will randomly change direction when the distance from the target is greater than this value.
    /// </summary>
    [Tooltip("The agent will randomly change direction when the distance from the target is greater than this value.")]
    [SerializeField] private float zigZagDistanceThreshold = 5.0f;

    /// <summary>
    /// The time in seconds the controller has to wait between flipping directions while its sides are being touched. (No homo)
    /// </summary>
    [Tooltip("The time in seconds the controller has to wait between flipping directions while its sides are being touched. (No homo)")]
    [SerializeField] private float flipOnSidesTriggerInterval = 0.3f;

    [Header("Attack")]

    /// <summary>
    /// Agent will attack when less than this distance from the player.
    /// </summary>
    [Tooltip("Agent will attack when less than this distance from the player.")]
    [SerializeField] private float attackRange = 3f;
    
    /// <summary>
    /// 
    /// </summary>
    [Tooltip("")]
    [SerializeField] private float attackRadius = 0.1f;

    [SerializeField]
    private LayerMask attackLayers = default;

    [SerializeField]
    private float attackFrequency = 1.0f;
    [SerializeField]
    private int attackDamageAmount = 10;


    #region Referenced components
    private Transform target;
    private Collider targetCollider;
    private Collider myCollider;
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
    public float lineOfSightRange = 10000;
    public float closeWakeRange = 10f;
    public LayerMask lineOfSightLayers = default;
    #endregion

    #region Flags
    public bool noTarget;
    #endregion

    

    private float lastFlipTime;

    private NavMeshPath path;
    private bool isAwake;

    private Vector3 moveDirection;

    [SerializeField]
    private float gravity = 30.0f;

    

    private float attackCooldownTime;
    private PlayerStats playerStats;
    private GameObject player;

    public UnityEvent onAttack;
    [SerializeField] private float attackDelay;

    [FormerlySerializedAs("attackFloatingSkullSoundEvent")]
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference attackSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance attackSoundInstance;



    #region Unity Messages
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        playerStats = player.GetComponent<PlayerStats>();
        targetCollider = target.GetComponent<Collider>();
        myCollider = GetComponent<Collider>();
        path = new NavMeshPath();
        elapsed = 0.0f;
        isMoving = false;
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
                direction = Quaternion.Euler(0, 30, 0) * direction;
            }
            else if (rand == 1)
            {
                direction = Quaternion.Euler(0, -30, 0) * direction;
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
        Vector3 attackDirection = direction;
        Vector3 closestPoint = targetCollider.ClosestPoint(attackOrigin);
        Vector3 playerDifference = closestPoint - attackOrigin;
        attackDirection.y = playerDifference.y;
        attackDirection.Normalize();
        return attackDirection;
    }

    private void UpdatePath()
    {
        // update the way to the goal every second.
        elapsed += Time.deltaTime * checkPathUpdateRate;

        // calculate path / update direction loop
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;

            if (PlayerInLineOfSight() && (IsTargetInCloseRange() || IsTargetInFront()))
            {
                isAwake = true;
            }

            if (isAwake)
            {
                if (!noTarget)
                {
                    if (playerStats.isDead)
                    {
                        int rand = Random.Range(0, 30);
                        if (rand == 0)
                        {
                            direction = Quaternion.Euler(0, 30, 0) * direction;
                        }
                        else if (rand == 1)
                        {
                            direction = Quaternion.Euler(0, -30, 0) * direction;
                        }
                    }
                    else
                    {
                        // try to calculate a path from the nav mesh
                        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
                        {
                            // calculate desired agent direction from the first two path points
                            direction = (corners[1] - corners[0]);
                            direction.y = 0;
                            direction.Normalize();

                            // begin moving
                            isMoving = true;

                            // random movement
                            RandomTurnLeftRight();
                        }
                        else
                        {
                            direction = target.position - transform.position;
                            direction.y = 0;
                            direction.Normalize();
                        }
                    }
                }
                else
                {
                    isAwake = false;
                    isMoving = false;
                }
            }
        }

        // attack loop
        if (isAwake)
        {
            // reduce timer
            if (attackCooldownTime > 0)
            {
                attackCooldownTime -= Time.deltaTime * attackFrequency;
            }

            // try attacking
            if (attackCooldownTime <= 0)
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
                            attackCooldownTime += 1f;
                            onAttack.Invoke();
                            StopAllCoroutines();
                            StartCoroutine(AttackWithDelay());
                        }

                    }
                }

                // if the cooldown timer was below zero and never attacked, reset to 0
                if (attackCooldownTime < 0)
                {
                    attackCooldownTime = 0;
                }
            }
        }
    }

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

    private void UpdateMovement()
    {
        float moveDirectionY = moveDirection.y;
        moveDirection = (isMoving && attackCooldownTime <= 0) ? speed * direction : Vector3.zero;
        moveDirection.y = moveDirectionY;

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        if (isMoving)
        {
            // Calculate rotation from direction
            transform.rotation = Quaternion.LookRotation(direction);

            // Check if the controller collided on its sides
            if ((characterController.collisionFlags & CollisionFlags.Sides) != 0)
            {
                if (Time.time > lastFlipTime + .4f)
                {
                    if (Physics.Raycast(new Vector3(myCollider.bounds.center.x, myCollider.bounds.min.y + 0.2f, myCollider.bounds.center.z), transform.forward, out RaycastHit wallHit, 5.0f, 1 << 0))
                    {
                        direction = Vector3.Reflect(direction, wallHit.normal);
                        lastFlipTime = Time.time;
                        float sign = 1;
                        if (Random.value < 0.5f) sign *= -1;
                        direction = Quaternion.Euler(0, Random.Range(30, 45) * sign, 0) * direction;
                    }
                }
            }
        }
    }
}