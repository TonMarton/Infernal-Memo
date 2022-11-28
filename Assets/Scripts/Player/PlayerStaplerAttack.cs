using UnityEngine;

public enum StaplerAttackState
{
    NotAttacking,
    Attacking,
}

[DisallowMultipleComponent]
public class PlayerStaplerAttack : MonoBehaviour, IPlayerAttack
{
    [Header("Hitbox volume")]
    [SerializeField] private StaplerHitbox staplerHitbox;
    
    [Header("Stats")]
    [SerializeField] private float startCanHitTime = 0.2f;
    [SerializeField] private float stopCanHitTime = 0.8f;
    [SerializeField] private float finishAttackTime = 1.2f;
    [SerializeField] private float cooldownBetweenAttacks = 5.0f;
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference attackSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance attackSoundInstance;

    private StaplerAttackState staplerAttackState = StaplerAttackState.NotAttacking;
    private float currentCooldown;

    private void Awake()
    {
        // disable the hitbox by default
        staplerHitbox.StopCanHit();
    }

    private void Update()
    {
        // update cooldown timer
        currentCooldown -= Time.deltaTime;
    }

    public void Attack()
    {
        // still cooling down?
        if (currentCooldown > 0.0f)
        {
            // can't attack yet
            return;
        }

        // already attacking?
        if (staplerAttackState == StaplerAttackState.Attacking)
        {
            // don't double attack
            return;
        }

        // mark as attacking
        staplerAttackState = StaplerAttackState.Attacking;

        // reset cooldown timer
        currentCooldown = cooldownBetweenAttacks;
        
        // play attack sound
        SoundUtils.PlaySound3D(ref attackSoundInstance, attackSoundEvent, gameObject);

        // start can hit after a delay (will use animation notify for this later)
        Invoke(nameof(StartCanHit), startCanHitTime);

        // stop can hit after a delay (will use animation notify for this later)
        Invoke(nameof(StopCanHit), stopCanHitTime);

        // finish attack after a delay (will use end of animation for this later)
        Invoke(nameof(FinishAttack), finishAttackTime);
    }

    private void StartCanHit()
    {
        // delegate to the hitbox
        staplerHitbox.StartCanHit();

        // check for collisions that already happened when the hitbox was just enabled
        staplerHitbox.CheckForCollisions();
    }

    private void StopCanHit()
    {
        // delegate to the hitbox
        staplerHitbox.StopCanHit();
    }

    private void FinishAttack()
    {
        // mark as not attacking
        staplerAttackState = StaplerAttackState.NotAttacking;
    }
}

public interface IPlayerAttack
{
    public void Attack();
}