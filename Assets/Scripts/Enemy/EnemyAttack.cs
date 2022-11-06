using UnityEngine;

public enum AttackState
{
    NotAttacking,
    Attacking,
    AttackFinished
}

[DisallowMultipleComponent]
public class EnemyAttack : MonoBehaviour
{
    // damage for the attack
    [SerializeField] protected int damage = 10;

    // Sounds
    private FMOD.Studio.EventInstance attackSoundInstance;

    public AttackState attackState { get; protected set; }

    public virtual void Attack()
    {
        // play hurt sound
        SoundUtils.PlaySound3D(attackSoundInstance, "Sfxs/Enemy/Enemy attack", gameObject);
    }

    public void NoLongerAttacking()
    {
        attackState = AttackState.NotAttacking;
    }
}