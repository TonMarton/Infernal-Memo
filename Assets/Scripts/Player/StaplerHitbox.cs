using UnityEngine;


// structure for knockback with a direction and a force
public struct Knockback
{
    public Vector3 direction;
    public float force;
}

[DisallowMultipleComponent]
public class StaplerHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 40f;
    [SerializeField] private float knockbackForce = 40f;
    private bool hitboxEnabled = false;
    private bool didDamageThisAttack = false;

    private void Awake()
    {
    }

    // on trigger collision
    private void OnTriggerEnter(Collider other)
    {
        // check collision
        CheckCollision(other);
    }

    public void CheckForCollisions()
    {
        // find all colliders in the overlap box
        var colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f);

        // check if any of them are enemies
        foreach (var collider in colliders)
        {
            CheckCollision(collider);
        }
    }

    private void CheckCollision(Component other)
    {
        // wasn't an enemy?
        if (!other.gameObject.CompareTag(Enemy.EnemyTag))
        {
            // ignore
            return;
        }

        var enemyStats = other.gameObject.GetComponentInParent<EnemyStats>();
        DamageEnemy(enemyStats);
    }

    private void DamageEnemy(EnemyStats enemyStats)
    {
        // can't hit?
        if (!CanHit())
        {
            // then do nothing
            return;
        }
        
        // remember that we did damage already this attack
        didDamageThisAttack = true;
        
        // build a knockback with a direction and a force
        var knockback = new Knockback
        {
            direction = (enemyStats.transform.position - transform.position).normalized,
            force = knockbackForce
        };

        // do damage to the enemy stats
        enemyStats.TakeDamage(damage, knockback);

        // disable the hitbox
        StopCanHit();
    }

    public void StartCanHit()
    {
        // didn't do damage yet this attack
        didDamageThisAttack = false;
        
        // enable the box collider
        hitboxEnabled = true;
    }

    public void StopCanHit()
    {
        // disable the box collider
        hitboxEnabled = false;
    }

    private bool CanHit()
    {
        return hitboxEnabled && !didDamageThisAttack;
    }

    private void OnDrawGizmos()
    {
        // can't hit?
        if (!CanHit())
        {
            // then don't draw
            return;
        }

        // draw the hitbox volume
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}