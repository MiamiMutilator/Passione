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
    public float TimeScale { get; set; }

    private IActivateable dash;
    private TimeSlow timeSlow;
    private bool dashing = false; // Dash is currently active
    private float timer = 0f;

    // TO REMOVE
    #region TEMPORARY
    public InputActionReference toggle;
    private bool toggleActive = false;
    private float startSlowTime = 0;
    private float slowTimer = 0;

    void ToggleTimeSlow()
    {
        if (toggleActive)
        {
            slowTimer += Time.deltaTime * TimeScale;
            //CheckTime<float>(slowTimer);

            if (slowTimer > timeSlowDuration)
            {
                DeactivateTimeSlow();
                toggleActive = false;
                //print("Timer lasted " + (Time.time - startSlowTime) + " seconds compared to the timeSlowDuration " + timeSlowDuration);
            }
        }

        if(toggle.action.triggered)
        {
            if (!toggleActive)
            {
                //Debug.Log("Time Slow Activated");
                toggleActive = true;
                ActivateTimeSlow();
                slowTimer = 0;
                startSlowTime = Time.time;
            }
        }
    }

    void CheckTime<T> (T value)
    {
        if (toggleActive)
        {
            Debug.Log("Value during Time Slow: " + value);
        }
        else
        {
            Debug.Log("Value outside of Time Slow: " + value);
        }
    }
    #endregion


    private void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.Enable();

        toggle.action.Enable(); // To Remove
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        dashAction.action.Disable();

        toggle.action.Disable(); // To Remove
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        dash = new Dash(this, characterController, dashVelocity);
        timeSlow = new TimeSlow(slowedTimeScale);
        TimeScale = Time.timeScale;

        if (!gameObject.CompareTag("Player")) Debug.LogWarning($"Give the Player tag to {gameObject.name}!");
    }

    private void Update()
    {
        ToggleTimeSlow(); // Testing Purposes

        UpdateTimeScale();
        ReadMoveInput();
        ApplyRotation();
        ApplyMovement();
        ApplyDash();
    }

    // If Time Slow is activated, counteract it with the reciprocal of the slowedTimeScale
    void UpdateTimeScale() => TimeScale = timeSlow.Activated ? 1 / slowedTimeScale : Time.timeScale; 
    

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

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * TimeScale);
    }

    private void ApplyMovement()
    {
        characterController.Move(moveSpeed * Time.deltaTime * TimeScale * moveDirection);
        lastDirection = moveDirection;
    }

    private void ApplyDash()
    {
        if (!dashing && dashAction.action.triggered)
        {
            // Start Dashing when input and can dash
            dashing = true;
            timer = 0;
            //timeSlow.OnActivation();
            //Invoke(nameof(DeactivateTimeSlow), timeSlowDuration / trueTimeScale );
        }

        if (dashing && timer < dashDuration)
        {
            // Smoothly dash for the duration
            timer += Time.deltaTime * TimeScale;
            dash.OnActivation();
        }
        else if (dashing)
        {
            // Dash reached duration
            timer = timer - dashDuration;
            dashing = false;
        }
    }

    void ActivateTimeSlow() => timeSlow.OnActivation();
    void DeactivateTimeSlow() => timeSlow.Deactivate();
    
}