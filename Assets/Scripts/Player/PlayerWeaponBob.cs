using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponBob : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float bobSpeed = 1.0f;
    [SerializeField] private Vector3 bobScale = Vector3.one;
    [SerializeField] private float preMultiplier = .25f;
    [SerializeField] private float finalMultiplier = 0.01f;


    private float timer;
    private Vector3 startPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        startPosition = weaponTransform.localPosition;
    }

    private void LateUpdate()
    {
        //// in the air?
        //if (!characterController.isGrounded)
        //{
        //    // don't play a sound
        //    return;
        //}

        // get character velocity with vertical portion cancelled out
        Vector3 velocity = characterController.velocity;
        velocity.y = 0;

        // calculate the footstep timer
        float amount = velocity.magnitude;
        timer += amount * bobSpeed * Time.deltaTime;

        //if (amount < 0.001f)
        //{
        //    timer = 0.5f;
        //}

        // footstep timer didn't hit yet?
        if (timer >= 1f)
        {
            // cycle the footstep timer
            timer %= 1f;

        }


        // apply transform
        Vector3 bobOffset = new Vector3(Mathf.Sin(timer * Mathf.PI * 2), Mathf.Sin(timer * Mathf.PI * 4), Mathf.Sin(timer * Mathf.PI * 4));
        bobOffset = Vector3.Scale(bobOffset, bobScale);
        weaponTransform.localPosition = startPosition + bobOffset * Mathf.Clamp01(amount * preMultiplier) * finalMultiplier;
    }
}
