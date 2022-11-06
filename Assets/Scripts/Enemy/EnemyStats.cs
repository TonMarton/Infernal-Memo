using UnityEngine;


public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int startHealth = 100;
    [SerializeField] private float health;
    [SerializeField] private float minTimeBetweenDamageSounds = 0.5f;

    // Sounds
    private FMOD.Studio.EventInstance hurtSoundInstance;
    private FMOD.Studio.EventInstance deathSoundInstance;

    private CapsuleCollider capsuleCollider;
    private float damageSoundCooldown;

    // Start is called before the first frame update
    private void Awake()
    {
        // get a reference to the child capsule collider
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();

        // set starting health
        health = startHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        damageSoundCooldown -= Time.deltaTime;
    }

    // take damage
    public void TakeDamage(float damage, Knockback? knockback)
    {
        // take damage
        health -= damage;
        
        // log damage
        Debug.Log("Took " + damage + " damage. Health is now " + health);

        // apply knockback if it was provided
        if (knockback != null)
        {
            // apply knockback to the enemy with the given vector and force
            capsuleCollider.GetComponent<Rigidbody>()
                .AddForce(knockback.Value.direction * knockback.Value.force, ForceMode.Impulse);
        }

        // should die?
        if (health <= 0)
        {
            // die
            Die();
        }
        else
        {
            // has it been long enough since the last damage sound?
            if (damageSoundCooldown <= 0)
            {
                // log playing damage sound
                Debug.Log("Playing damage sound");
                // play hurt sound
                SoundUtils.PlaySound3D(hurtSoundInstance, "Sfxs/Enemy/Enemy Damage", gameObject);
                
                // reset the damage sound cooldown
                damageSoundCooldown = minTimeBetweenDamageSounds;
            }
        }
    }

    // die
    private void Die()
    {
        // TODO: play death sound with fmod

        // play death sound
        SoundUtils.PlaySound3D(hurtSoundInstance, "Sfxs/Enemy/Enemy death", gameObject);

        // disable enemy
        gameObject.SetActive(false);
    }
}