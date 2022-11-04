using System.Collections;
using UnityEngine;

public class RangedEnemyAttack : EnemyAttack
{
    // max range for the attack
    [SerializeField] private float range = 20f;

    // attack method
    public override void Attack()
    {
        // remember that we're attacking
        attacking = true;
        
        Debug.Log("Attacking");
        
        // do a line trace attack forward
        if (!Physics.Raycast(transform.position, transform.forward, out var hit, range))
        {
            // attack whiffed
            Debug.Log("Attack whiffed");
            return;
        }

        // raycast hit the player
        if (hit.collider.CompareTag(PlayerBase.PLAYER_TAG))
        {
            // apply damage
            Debug.Log("Hit player");
            hit.collider.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
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
        // log that we're done attacking
        Debug.Log("Done attacking");
        attacking = false;
    }
}