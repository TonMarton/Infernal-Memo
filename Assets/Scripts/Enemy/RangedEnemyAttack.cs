using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class RangedEnemyAttack : EnemyAttack
{
    // max range for the attack
    [SerializeField] private float range = 20f;

    // attack method
    public override void Attack()
    {
        // remember that we're attacking
        attackState = AttackState.Attacking;
        
        // do a line trace attack forward
        if (!Physics.Raycast(transform.position, transform.forward, out var hit, range))
        {
            // attack whiffed
            return;
        }

        // raycast hit the player
        if (hit.collider.CompareTag(PlayerBase.PLAYER_TAG))
        {
            // apply damage
            hit.collider.gameObject.GetComponent<PlayerStats>().UpdateHealth(-damage);
            
            // play the damage sound for this enemy on the player
            SoundUtils.PlaySound3D(damageSoundInstance, damageSoundEvent, hit.collider.gameObject);
        }
        
        // TODO: set attacking = false at the end of attack animation
        // temporarily set attacking = false after a delay with coroutine
        StartCoroutine(ResetAttacking());
    }
    
    // TODO: this is temporary remove it once we get anim notify states in
    // coroutine set attacking to false after a delay
    private IEnumerator ResetAttacking()
    {
        yield return new WaitForSeconds(2f);
        attackState = AttackState.AttackFinished;
    }
}