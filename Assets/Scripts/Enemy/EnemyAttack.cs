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
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference attackSoundEvent;
    [SerializeField] protected FMODUnity.EventReference damageSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance attackSoundInstance;
    protected FMOD.Studio.EventInstance damageSoundInstance;

    public AttackState attackState { get; protected set; }

    public virtual void Attack()
    {
        // play hurt sound
        SoundUtils.PlaySound3D(ref attackSoundInstance, attackSoundEvent, gameObject);
    }

    public void NoLongerAttacking()
    {
        attackState = AttackState.NotAttacking;
    }
}