using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private Camera playerCamera;

    private CharacterController _characterController;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

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
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Called when the Move input event triggers
        moveInput = context.ReadValue<Vector2>();
        moveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);

        //Debug.Log("Move called with value " + _input);
        //Debug.Log("Direction set to " + _direction);
    }
}
