using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;
    public Camera playerCamera;
    public InputActionReference moveAction;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    [HideInInspector] public Vector3 lastDirection;

    [Header("Dash")]
    public float dashVelocity = 2f;
    [Tooltip("Duration in seconds")]
    public float dashDuration = 0.1f;
    [Tooltip("Duration in seconds")]
    public float timeSlowDuration = 1.5f;
    [Tooltip("What the time scale gets set to when Time Slow activates. 1 is normal speed, 0 is paused")]
    public float slowedTimeScale = 0.5f;
    public InputActionReference dashAction;

    private IActivateable dash;
    private TimeSlow timeSlow;
    private bool dashing = false; // Dash is currently active
    private float timer = 0f;

    private void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.Enable();
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        dashAction.action.Disable();
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        dash = new Dash(this, characterController, dashVelocity);
        timeSlow = new TimeSlow(slowedTimeScale, timeSlowDuration);

        if (!gameObject.CompareTag("Player")) Debug.LogWarning($"Give the Player tag to {gameObject.name}!");
    }

    private void Update()
    {
        ReadMoveInput();
        ApplyRotation();
        ApplyMovement();
        ApplyDash();
    }

    void ReadMoveInput()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        moveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);
    }

    private void ApplyRotation()
    {
        if (moveInput.sqrMagnitude == 0) return;

        moveDirection = Quaternion.Euler(0.0f, playerCamera.transform.eulerAngles.y, 0.0f) * new Vector3(moveInput.x, 0.0f, moveInput.y);
        var targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        characterController.Move(moveSpeed * Time.deltaTime * moveDirection);
        lastDirection = moveDirection;
    }

    private void ApplyDash()
    {
        if (!dashing && dashAction.action.triggered)
        {
            // Start Dashing when input and can dash
            dashing = true;
            timer = 0;
            timeSlow.OnActivation();
            Invoke(nameof(DeactivateTimeSlow), 1 + (1/timeSlowDuration));
        }

        if (dashing && timer < dashDuration)
        {
            // Smoothly dash for the duration
            timer += Time.deltaTime;
            dash.OnActivation();
        }
        else if (dashing)
        {
            // Dash reached duration
            timer = 0;
            dashing = false;
        }
    }

    void DeactivateTimeSlow()
    {
        timeSlow.Deactivate();
    }
}