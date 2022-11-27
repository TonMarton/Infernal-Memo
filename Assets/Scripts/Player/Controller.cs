using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public class Controller : MonoBehaviour
{
    private bool canMove { get; set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !crouchAnimation && characterController.isGrounded;

    [FormerlySerializedAs("_canSprint")] [Header("Functional")] [SerializeField]
    private bool canSprint = true;

    [FormerlySerializedAs("_canJump")] [SerializeField]
    private bool canJump = true;

    [FormerlySerializedAs("_canCrouch")] [SerializeField]
    private bool canCrouch = true;

    [FormerlySerializedAs("_canShakeScreen")] [SerializeField]
    private bool canShakeScreen = true;

    [FormerlySerializedAs("_sprintKey")] [Header("Controls")] [SerializeField]
    private KeyCode sprintKey = KeyCode.LeftShift;

    [FormerlySerializedAs("_jumpKey")] [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [FormerlySerializedAs("_crouchKey")] [SerializeField]
    private KeyCode crouchKey = KeyCode.C;

    [FormerlySerializedAs("_walkSpeed")] [Header("Movement Params")] [SerializeField]
    private float walkSpeed = 3.65f;

    [FormerlySerializedAs("_sprintSpeed")] [SerializeField]
    private float sprintSpeed = 6.15f;

    [FormerlySerializedAs("_crouchSpeed")] [SerializeField]
    private float crouchSpeed = 1.85f;

    [FormerlySerializedAs("_lookSpeedX")] [Header("Look Params")] [SerializeField, Range(1, 10)]
    private float lookSpeedX = 2.0f;

    [FormerlySerializedAs("_lookSpeedY")] [SerializeField, Range(1, 10)]
    private float lookSpeedY = 2.0f;

    [FormerlySerializedAs("_upperLookLimit")] [SerializeField, Range(1, 180)]
    private float upperLookLimit = 80.0f;

    [FormerlySerializedAs("_lowerLookLimit")] [SerializeField, Range(1, 180)]
    private float lowerLookLimit = 80.0f;

    [FormerlySerializedAs("_jumpForce")] [Header("Force/Gravity")] [SerializeField]
    private float jumpForce = 8.0f;

    [SerializeField] private float _gravity = 30.0f;

    [Header("Crouch")] [SerializeField] private float _crouchHeight = .5f;

    [FormerlySerializedAs("_standingHeight")] [SerializeField]
    private float standingHeight = 2f;

    [FormerlySerializedAs("_timeToCrouch")] [SerializeField]
    private float timeToCrouch = .25f;

    [FormerlySerializedAs("_crouchingCenter")] [SerializeField]
    private Vector3 crouchingCenter = new Vector3(0, .5f, 0);

    [FormerlySerializedAs("_standingCenter")] [SerializeField]
    private Vector3 standingCenter = new Vector3(0, 0, 0);

    private bool isCrouching;
    private bool crouchAnimation;

    [FormerlySerializedAs("_walkShakeSpeed")] [Header("Screenshake")] [SerializeField]
    private float walkShakeSpeed = 14f;

    [FormerlySerializedAs("_walkShakeAmount")] [SerializeField]
    private float walkShakeAmount = .05f;

    [FormerlySerializedAs("_sprintShakeSpeed")] [SerializeField]
    private float sprintShakeSpeed = 18f;

    [FormerlySerializedAs("_sprintShakeAmount")] [SerializeField]
    private float sprintShakeAmount = .11f;

    [FormerlySerializedAs("_crouchShakeSpeed")] [SerializeField]
    private float crouchShakeSpeed = 8f;

    [FormerlySerializedAs("_crouchShakeAmount")] [SerializeField]
    private float crouchShakeAmount = .025f;

    [SerializeField]
    private float damageShakeSpeed = 25f;

    [SerializeField]
    private float damageShakeDuration = 0.2f;

    [SerializeField] private PauseMenu PauseMenuScript;

    [SerializeField]
    private AnimationCurve damageShakeCurve;

    [Header("Sound")] [SerializeField] private FMODUnity.EventReference jumpSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance jumpSoundInstance;

    private float defaultYPos = 0;
    private float timer;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    public bool damageTaken = false;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        // remember the starting mouse location
        defaultYPos = playerCamera.transform.localPosition.y;

        // hijack the cursor for player look by locking the cursor and hiding it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Time.timeScale == 1)
        {
            // Can't move?
            if (!canMove)
            {
                // then do nothing
                return;
            }

            // move
            MovementInput();

            // look
            MouseLook();

            // can jump?
            if (canJump)
            {
                // then jump
                Jump();
            }

            // can crouch?
            if (canCrouch)
            {
                // then crouch
                Crouch();
            }

            // can shake screen?
            if (canShakeScreen)
            {
                if (damageTaken)
                {
                    StartCoroutine(DamageScreenShake());
                }
                else
                {
                    // then shake the screen
                    ScreenShake();
                }
            }

            FinalMovements();

            CheckCeilingCollisions();
        }
    }

    private void MovementInput()
    {
        currentInput =
            new Vector2(
                (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
                (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        var moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                        (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void MouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void Jump()
    {
        // can't jump?
        if (!ShouldJump || isCrouching)
        {
            // do nothing
            return;
        }

        // apply jump force
        moveDirection.y = jumpForce;

        // play jump sound
        SoundUtils.PlaySound3D(jumpSoundInstance, jumpSoundEvent, gameObject);
    }

    private void Crouch()
    {
        // shouldn't crouch?
        if (!ShouldCrouch)
        {
            // do nothing
            return;
        }

        // crouch
        StartCoroutine(CrouchStand());
    }

    private IEnumerator DamageScreenShake() {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < damageShakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = damageShakeCurve.Evaluate(elapsedTime / damageShakeDuration);
            transform.position = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPos;
        damageTaken = false;
    }

    private void ScreenShake()
    {
        if (!characterController.isGrounded) return;

        if (!(Mathf.Abs(moveDirection.x) > 0.1f) && !(Mathf.Abs(moveDirection.z) > 0.1f))
        {
            return;
        }

        float shakeAmount = walkShakeAmount;
        float shakeSpeed = walkShakeSpeed;

        if (isCrouching)
        {
            shakeSpeed = crouchShakeSpeed;
            shakeAmount = crouchShakeAmount;
        }
        else if (IsSprinting)
        {
            shakeSpeed = sprintShakeSpeed;
            shakeAmount = sprintShakeAmount;
        }

        timer += Time.deltaTime * shakeSpeed;

        playerCamera.transform.localPosition = new Vector3(
            playerCamera.transform.localPosition.x,
            defaultYPos + Mathf.Sin(timer) * shakeAmount,
            playerCamera.transform.localPosition.z);
    }

    private void FinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= _gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void CheckCeilingCollisions()
    {
        if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            // Set vertical movement to 0 when character hits the ceiling
            if (moveDirection.y > 0)
            {
                moveDirection.y = 0;
            }
        }
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1.5f))
            yield break;

        crouchAnimation = true;

        float timeElapsed = 0;
        var targetHeight = isCrouching ? standingHeight : _crouchHeight;
        var currentHeight = characterController.height;
        var targetCenter = isCrouching ? standingCenter : crouchingCenter;
        var currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;
        crouchAnimation = false;
    }

    public void TeleportToPositionMaintainingRelativePosition(Vector3 destination, Quaternion rotationDifference) {
        characterController.enabled = false;
        gameObject.transform.position = destination;
        gameObject.transform.rotation *= rotationDifference;
        characterController.enabled = true;
        Physics.SyncTransforms();
        Debug.Log("Player teleported to new location");
    }
}