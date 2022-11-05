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
    [SerializeField] private float knockbackForce = 10f;
    private BoxCollider boxCollider;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // on trigger collision
    private void OnTriggerEnter(Collider other)
    {
        // debug log the other collider tag, collided with tag
        Debug.Log("Collided with " + other.tag);
        
        // wasn't an enemy?
        if (!other.gameObject.CompareTag(Enemy.EnemyTag))
        {
            // ignore
            return;
        }

        Debug.Log("!!! Damage " + other.name);

        // build a knockback with a direction and a force
        var knockback = new Knockback
        {
            direction = (other.transform.position - transform.position).normalized,
            force = knockbackForce
        };

        // do damage to the enemy stats
        other.gameObject.GetComponentInParent<EnemyStats>().TakeDamage(damage, knockback);

        // disable the hitbox
        StopCanHit();
    }

    public void StartCanHit()
    {
        // enable the box collider
        boxCollider.enabled = true;
    }

    public void StopCanHit()
    {
        // disable the box collider
        boxCollider.enabled = false;
    }

    private bool CanHit()
    {
        return boxCollider && boxCollider.enabled;
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