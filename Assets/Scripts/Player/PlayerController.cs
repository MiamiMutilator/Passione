using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
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
    [Tooltip("Duration of the Dash in seconds")]
    public float dashDuration = 0.1f;
    [Tooltip("The number of dashes that can be performed before undergoing cooldown")]
    public int maxConsecutiveDashes = 2;
    [Tooltip("Duration of the dash cooldown in seconds")]
    public float dashCooldown = 0.5f;
    [Tooltip("Determines how long the dodge window is active for. Added on top of dash duration")]
    public float evadeBuffer = 0.3f;

    private IActivateable dash;
    private int currentDashes = 0;
    private bool canDash = true;
    private PunchHandler punchHandler;
    [HideInInspector] public bool dashing = false; // Dash is currently active.
    private bool isDodging = false;
    private float totalDodgeTime = 0;
    private float dodgeTimer = 0;

    [Header("Time Slow")]
    [Tooltip("Duration of the Time Slow in seconds")]
    public float timeSlowDuration = 1.5f;
    [Tooltip("What the time scale gets set to when Time Slow activates. 1 is normal speed, 0 is paused")]
    public float slowedTimeScale = 0.5f;
    public InputActionReference dashAction;
    public float TimeScale { get; set; }

    private TimeSlow timeSlow;
    private float dashTimer = 0f;
    private bool timeSlowActivated = false;
    private bool toggleActive = false;
    private float slowTimer = 0;

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
        timeSlow = new TimeSlow(slowedTimeScale);
        TimeScale = Time.timeScale;
        punchHandler = GetComponent<PunchHandler>();

        if (!gameObject.CompareTag("Player")) Debug.LogWarning($"Give the Player tag to {gameObject.name}!");
    }

    private void Update()
    {
        ApplyTimeSlow();
        UpdateTimeScale();
        ReadMoveInput();
        ApplyRotation();
        ApplyMovement();
        ApplyDash();
        ApplyDodge();
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
        if (punchHandler.IsPunching()) return;

        if (canDash && currentDashes == maxConsecutiveDashes && !dashing)
        {
            // Dashed consecutively the max number of times. Start cooldown
            StartCoroutine(DashCooldown());
        }
        else if (canDash && currentDashes < maxConsecutiveDashes)
        {
            if (!dashing && dashAction.action.triggered)
            {
                // Start Dashing when input and can dash
                dashing = true;
                totalDodgeTime += (dashDuration + evadeBuffer); // Evade enemy attacks for the duration
                dashTimer = 0;
            }

            if (dashing && dashTimer < dashDuration)
            {
                // Smoothly dash for the duration
                dashTimer += Time.deltaTime * TimeScale;
                dash.OnActivation();
            }
            else if (dashing)
            {
                // Dash reached duration
                dashTimer = 0;
                dashing = false;
                currentDashes++;
                Debug.Log("Dash Complete. Current Dashes: " + currentDashes);
            }
        }
    }

    void ApplyDodge()
    {
        if (totalDodgeTime > 0 && !isDodging)
        {
            Debug.Log("Started dodging attacks.");
            isDodging = true;
            dodgeTimer = 0;
        }

        if (isDodging && dodgeTimer < totalDodgeTime)
        {
            dodgeTimer += Time.deltaTime * TimeScale;
        }
        else if (isDodging)
        {
            Debug.Log("Stopped dodging attacks");
            totalDodgeTime = 0;
            dodgeTimer = 0;
            isDodging = false;
        }
    }

    IEnumerator DashCooldown()
    {
        Debug.Log("Starting Dash Cooldown");
        canDash = false;
        yield return new WaitForSeconds(dashCooldown / TimeScale);
        Debug.Log("Dash Cooldown finished");
        canDash = true;
        currentDashes = 0;
    }

    public void OnEvade() => timeSlowActivated = true;

    void ApplyTimeSlow()
    {
        if (timeSlowActivated && !toggleActive)
        {
            toggleActive = true;
            ActivateTimeSlow();
            slowTimer = 0;
        }
        else if (timeSlowActivated && toggleActive)
        {
            slowTimer += Time.deltaTime * TimeScale;

            if (slowTimer > timeSlowDuration)
            {
                DeactivateTimeSlow();
                toggleActive = false;
                timeSlowActivated = false;
            }
        }
    }

    void ActivateTimeSlow() => timeSlow.OnActivation();
    void DeactivateTimeSlow() => timeSlow.Deactivate();

    public bool IsDodging() => isDodging;
}