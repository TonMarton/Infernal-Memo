using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    public bool _canMove { get; private set; } = true;
    private bool _IsSprinting => _canSprint && Input.GetKey(_sprintKey);
    private bool _ShouldJump => Input.GetKeyDown(_jumpKey) && _characterController.isGrounded;
    private bool _ShouldCrouch => Input.GetKeyDown(_crouchKey) && !_crouchAnimation && _characterController.isGrounded;

    [Header("Functional")]
    [SerializeField] private bool _canSprint = true;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canCrouch = true;
    [SerializeField] private bool _canShakeScreen = true;

    [Header("Controls")]
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _crouchKey = KeyCode.C;

    [Header("Movement Params")]
    [SerializeField] private float _walkSpeed = 3.65f;
    [SerializeField] private float _sprintSpeed = 6.15f;
    [SerializeField] private float _crouchSpeed = 1.85f;

    [Header("Look Params")]
    [SerializeField, Range(1, 10)] private float _lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float _lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float _upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float _lowerLookLimit = 80.0f;

    [Header("Force/Gravity")]
    [SerializeField] private float _jumpForce = 8.0f;
    [SerializeField] private float _gravity = 30.0f;

    [Header("Crouch")]
    [SerializeField] private float _crouchHeight = .5f;
    [SerializeField] private float _standingHeight = 2f;
    [SerializeField] private float _timeToCrouch = .25f;
    [SerializeField] private Vector3 _crouchingCenter = new Vector3(0, .5f, 0);
    [SerializeField] private Vector3 _standingCenter = new Vector3(0, 0, 0);
    private bool _isCrouching;
    private bool _crouchAnimation;

    [Header("Screenshake")]
    [SerializeField] private float _walkShakeSpeed = 14f;
    [SerializeField] private float _walkShakeAmount = .05f;
    [SerializeField] private float _sprintShakeSpeed = 18f;
    [SerializeField] private float _sprintShakeAmount = .11f;
    [SerializeField] private float _crouchShakeSpeed = 8f;
    [SerializeField] private float _crouchShakeAmount = .025f;
    private float defaulyYPos = 0;
    private float timer;

    private Camera _playerCamera;
    private CharacterController _characterController;

    private Vector3 _moveDirection;
    private Vector2 _currentInput;

    private float _rotationX = 0;

    void Awake()
    {
        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponent<CharacterController>();
        defaulyYPos = _playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (_canMove)
        {
            MovementInput();
            MouseLook();

            if (_canJump)
                Jump();

            if (_canCrouch)
                Crouch();

            if (_canShakeScreen)
                ScreenShake();

            FinalMovements();
        }
    }

    private void MovementInput()
    {
        _currentInput = new Vector2((_isCrouching ? _crouchSpeed : _IsSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Vertical"), (_isCrouching ? _crouchSpeed : _IsSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = _moveDirection.y;
        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);
        _moveDirection.y = moveDirectionY;
    }

    private void MouseLook()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * _lookSpeedY;
        _rotationX = Mathf.Clamp(_rotationX, -_upperLookLimit, _lowerLookLimit);
        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeedX, 0);
    }

    private void Jump()
    {
        if (_ShouldJump && !_isCrouching)
            _moveDirection.y = _jumpForce;
    }

    private void Crouch()
    {
        if (_ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void ScreenShake()
    {
        if (!_characterController.isGrounded) return;

        if (Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (_isCrouching ? _crouchShakeSpeed : _IsSprinting ? _sprintShakeSpeed : _walkShakeSpeed);
            _playerCamera.transform.localPosition = new Vector3(
                _playerCamera.transform.localPosition.x, defaulyYPos + Mathf.Sin(timer) * (_isCrouching ? _crouchShakeAmount : _IsSprinting ? _sprintShakeAmount : _walkShakeAmount),
                _playerCamera.transform.localPosition.z);
        }
    }

    private void FinalMovements()
    {
        if (!_characterController.isGrounded)
            _moveDirection.y -= _gravity * Time.deltaTime;

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (_isCrouching && Physics.Raycast(_playerCamera.transform.position, Vector3.up, 1.5f))
            yield break;

        _crouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = _isCrouching ? _standingHeight : _crouchHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = _isCrouching ? _standingCenter : _crouchingCenter;
        Vector3 currentCenter = _characterController.center;

        while(timeElapsed < _timeToCrouch)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _characterController.height = targetHeight;
        _characterController.center = targetCenter;

        _isCrouching = !_isCrouching;
        _crouchAnimation = false;
    }
}
