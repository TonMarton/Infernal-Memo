using UnityEngine;
using UnityEngine.AI;


// Interface for if the enemy should attack or not
public interface IEnemyShouldAttack
{
    bool ShouldAttack();
    GameObject GetPlayer();

    private bool PlayerInRange()
    {
        throw new System.NotImplementedException();
    }

    private bool PlayerInLineOfSight()
    {
        throw new System.NotImplementedException();
    }
}


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyAI : MonoBehaviour, IEnemyShouldAttack
{
    [SerializeField] private float minAttackRange = 5f;
    [SerializeField] private float lineOfSightRange = 10f;

    private GameObject player;
    private NavMeshAgent agent;
    private protected EnemyAttack attack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<EnemyAttack>();

        // target the player by default
        player = GameObject.Find("Player");
        // throw error if target not found
        if (player == null)
        {
            Debug.LogError("Target not found!");
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private protected void MoveTowardPlayer()
    {
        // set the agent destination to target
        agent.SetDestination(player.transform.position);
    }

    public bool ShouldAttack()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetPlayer()
    {
        return player;
    }


    bool IEnemyShouldAttack.ShouldAttack()
    {
        // Attack if player is within range and in line of sight
        return PlayerInRange() && PlayerInLineOfSight();
    }

    private bool PlayerInRange()
    {
        // get the range to the player
        var range = Vector3.Distance(transform.position, GetPlayer().transform.position);

        // check if player is within range
        return range <= minAttackRange;
    }

    private bool PlayerInLineOfSight()
    {
        // do a raycast to check whether player is in line of sight of the enemy
        var rayDirection = player.transform.position - transform.position;
        if (!Physics.Raycast(transform.position, rayDirection, out var hit, lineOfSightRange))
        {
            // raycast didn't hit anything so player isn't in line of sight
            return false;
        }

        // did the raycast hits the player?
        return hit.collider.gameObject == player;
    }
}