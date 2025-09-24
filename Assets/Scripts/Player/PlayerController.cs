using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;
    public Camera playerCamera;

    private CharacterController _characterController;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    [HideInInspector] public Vector3 lastDirection;

    [Header("Dash")]
    [Range(0, 10)]
    public float dashVelocity;

    private IAction dash;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        dash = new Dash(this, GetComponent<CharacterController>(), dashVelocity);

        if (!gameObject.CompareTag("Player")) Debug.LogWarning($"Give the Player tag to {gameObject.name}!");
    }

    private void Update()
    {
        ApplyRotation();
        ApplyMovement();
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
        _characterController.Move(moveSpeed * Time.deltaTime * moveDirection);
        lastDirection = moveDirection;
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Tied to "Move" InputActionMap
        moveInput = context.ReadValue<Vector2>();
        moveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);

        //Debug.Log("Move called with value " + _input);
        //Debug.Log("Direction set to " + _direction);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        // Tied to "Dash" InputActionMap
        //print("Dash called with value " + context.action.triggered);
        if (context.action.triggered) dash.OnActivation();
    }
}
