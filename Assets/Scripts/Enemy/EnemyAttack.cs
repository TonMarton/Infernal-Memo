using UnityEngine;

public enum AttackState
{
    NotAttacking,
    Attacking,
    AttackFinished
}

public class EnemyAttack : MonoBehaviour
{
    // damage for the attack
    [SerializeField] protected int damage = 10;

    public AttackState attackState { get; protected set; }

    public virtual void Attack()
    {
        // TODO: play attack sound with Fmod
    }

    public void NoLongerAttacking()
    {
        attackState = AttackState.NotAttacking;
    }
}