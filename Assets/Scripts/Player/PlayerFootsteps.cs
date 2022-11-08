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

    private void Update()
    {
        Vector3 velocity = characterController.velocity;
        velocity.y = 0;
        float amount = velocity.magnitude;
        timer += amount * stepSpeed;

        if (timer >= 1f && characterController.isGrounded)
        {
            timer %= 1f;
            PlayFootstepSound();
        }
    }

    public void PlayFootstepSound()
    {
        //Debug.Log("Step");
        
        SoundUtils.PlaySound3D(footstepSoundInstance, footstepSoundEvent, gameObject);
    }
}
