using System;
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
    [Range(0, 20)]
    public float dashVelocity = 2f;
    public InputActionReference dashAction;

    private IAction dash;

    private void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.started += Dash;
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        dashAction.action.started -= Dash;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        dash = new Dash(this, characterController, dashVelocity);

        if (!gameObject.CompareTag("Player")) Debug.LogWarning($"Give the Player tag to {gameObject.name}!");
    }

    private void Update()
    {
        ReadMoveInput();
        ApplyRotation();
        ApplyMovement();
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

    public void Dash(InputAction.CallbackContext context)
    {
        //print("Dash called with value " + context.action.triggered);
        if (context.action.triggered) dash.OnActivation();
    }
}