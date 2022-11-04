using UnityEngine;

public class RangedEnemyAttack : EnemyAttack
{
    // max range for the attack
    [SerializeField] private float range = 10f;

    // attack method
    public override void Attack()
    {
        // remember that we're attacking
        attacking = true;
        
        // do a line trace attack at the player
        if (!Physics.Raycast(transform.position, transform.forward, out var hit, range))
        {
            // attack whiffed
            return;
        }

        // raycast hit the player
        if (hit.collider.gameObject.tag == PlayerBase.PLAYER_TAG)
        {
            // apply damage
            hit.collider.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
        }
        
        // TODO: set attacking = false at the end of attack animation
    }
}