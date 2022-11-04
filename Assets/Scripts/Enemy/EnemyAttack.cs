using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // damage for the attack
    [SerializeField]
    protected int damage = 10;

    protected bool attacking = false;
    
    public virtual void Attack()
    {
        // TODO: play attack sound with Fmod
    }

    public bool IsAttacking()
    {
        return attacking;
    }

    public bool IsDoneAttacking()
    {
        return !IsAttacking();
    }
}