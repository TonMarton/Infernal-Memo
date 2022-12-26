using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyFootsteps : MonoBehaviour
{
    [SerializeField] private float stepSpeed = 1.0f;

    [Header("Sound")] [SerializeField] private FMODUnity.EventReference footstepSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance footstepSoundInstance;

    private CharacterController characterController;
    private float timer;
    private EnemyStats enemyStats;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        enemyStats = GetComponent<EnemyStats>();
    }

    private void LateUpdate()
    {
        if (enemyStats.isDead)
        {
            return;
        }

        // in the air?
        if (!characterController.isGrounded)
        {
            // don't play a sound
            return;
        }

        // get character velocity with vertical portion cancelled out
        Vector3 velocity = characterController.velocity;
        velocity.y = 0;
        
        // calculate the footstep timer
        float amount = Mathf.Sqrt(velocity.magnitude);
        timer += amount * stepSpeed * Time.deltaTime;

        // footstep timer didn't hit yet?
        if (!(timer >= 1f))
        {
            // then do nothing
            return;
        }

        // cycle the footstep timer
        timer %= 1f;
        
        // play footstep sound
        try
        {
            PlayFootstepSound();
        }
        catch (System.Exception)
        {
        }
    }

    private void PlayFootstepSound()
    {
        SoundUtils.PlaySound3D(ref footstepSoundInstance, footstepSoundEvent, gameObject);
    }
}