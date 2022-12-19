using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int startHealth = 100;
    [SerializeField] private float health;
    [SerializeField] private float minTimeBetweenDamageSounds = 0.5f;
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference hurtSoundEvent;
    [SerializeField] private FMODUnity.EventReference deathSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance hurtSoundInstance;
    private FMOD.Studio.EventInstance deathSoundInstance;

    private CapsuleCollider capsuleCollider;
    private float damageSoundCooldown;

    NavAgent navAgent;

    // Start is called before the first frame update
    private void Awake()
    {
        // get a reference to the child capsule collider
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();

        navAgent = GetComponent<NavAgent>();

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

        if (navAgent != null) navAgent.WakeUp();
        
        // log how much damage we took and our current health
        Debug.Log("Enemy took " + damage + " damage. Health is now " + health);

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
                // play hurt sound
                SoundUtils.PlaySound3D(ref hurtSoundInstance, hurtSoundEvent, gameObject);

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
        SoundUtils.PlaySound3D(ref hurtSoundInstance, deathSoundEvent, gameObject);

        gameObject.GetComponentInParent<LevelManager>().DecreaseEnemyCount();

        // disable enemy
        gameObject.SetActive(false);
    }
}