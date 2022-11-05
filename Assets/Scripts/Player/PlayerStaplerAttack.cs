using UnityEngine;

public enum StaplerAttackState
{
    NotAttacking,
    Attacking,
}

[DisallowMultipleComponent]
public class PlayerStaplerAttack : MonoBehaviour, IPlayerAttack
{
    [SerializeField] private StaplerHitbox staplerHitbox;
    [SerializeField] private float startCanHitTime = 0.2f;
    [SerializeField] private float stopCanHitTime = 0.8f;
    [SerializeField] private float finishAttackTime = 1.2f;
    private StaplerAttackState staplerAttackState = StaplerAttackState.NotAttacking;

    private void Awake()
    {
        // disable the hitbox by default
        staplerHitbox.StopCanHit();
    }

    public void Attack()
    {
        // already attacking?
        if (staplerAttackState == StaplerAttackState.Attacking)
        {
            // don't double attack
            return;
        }
        
        // mark as attacking
        staplerAttackState = StaplerAttackState.Attacking;
        
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