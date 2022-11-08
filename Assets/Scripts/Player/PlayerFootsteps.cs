using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private float stepSpeed = 1.0f;

    private CharacterController characterController;
    private float timer;

    [Header("Sound")]
    [SerializeField]
    private FMODUnity.EventReference footstepSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance footstepSoundInstance;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {

        if (characterController.isGrounded)
        {
            Vector3 velocity = characterController.velocity;
            velocity.y = 0;
            float amount = velocity.magnitude;
            timer += amount * stepSpeed * Time.deltaTime;

            if (timer >= 1f)
            {
                timer %= 1f;
                try
                {
                    PlayFootstepSound();
                }
                catch (System.Exception)
                {
                }
            }
        }

    }

    public void PlayFootstepSound()
    {
        Debug.Log("<footstep>"); // print to console until sounds work
        SoundUtils.PlaySound3D(footstepSoundInstance, footstepSoundEvent, gameObject);
    }
}
