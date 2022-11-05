using UnityEngine;


public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int startHealth = 100;
    [SerializeField] private float health;
    private CapsuleCollider capsuleCollider;

    // Start is called before the first frame update
    private void Awake()
    {
        // get a reference to the child capsule collider
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        
        // set starting health
        health = startHealth;
    }

    // take damage
    public void TakeDamage(float damage, Knockback? knockback)
    {
        // take damage
        health -= damage;
        
        Debug.Log("Enemy took " + damage + " damage. Health is now " + health);
        
        // apply knockback if it was provided
        if (knockback != null)
        {
            // apply knockback to the enemy with the given vector and force
            capsuleCollider.GetComponent<Rigidbody>().AddForce(knockback.Value.direction * knockback.Value.force, ForceMode.Impulse);
        }

        // should die?
        if (health <= 0)
        {
            // die
            Die();
        }
        else
        {
            // TOD: play take damage sound with fmod
        }
    }

    // die
    private void Die()
    {
        // TODO: play death sound with fmod
        
        Debug.Log("---- Enemy died");

        // disable enemy
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}