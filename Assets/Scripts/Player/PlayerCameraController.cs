using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerCameraController : MonoBehaviour
{
    public InputActionReference lookAction;
    [SerializeField] private Transform player;
    [SerializeField] private MouseSensitivity mouseSensitivity;
    [SerializeField] private MouseSensitivity controllerSensitivity;
    [SerializeField] private CameraAngle cameraAngle;

    private CameraRotation cameraRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateMouseSensitivity(); //
        controllerSensitivity.invertHorizontal = true; //
    }

    private void OnEnable()
    {
        lookAction.action.Enable();
    }
    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    private void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        
        MouseSensitivity currentSensitivity = controllerSensitivity; //
        if (lookAction.action.activeControl != null)
        {

            if (lookAction.action.activeControl.device.name.Equals("Mouse"))
            {
                currentSensitivity = mouseSensitivity;
            }
        }
        cameraRotation.Yaw += lookInput.x * currentSensitivity.horizontal * BoolToInt(currentSensitivity.invertHorizontal) * Time.deltaTime;
        cameraRotation.Pitch += lookInput.y * currentSensitivity.vertical * BoolToInt(currentSensitivity.invertVertical) * Time.deltaTime;
        cameraRotation.Pitch = Mathf.Clamp(cameraRotation.Pitch, cameraAngle.min, cameraAngle.max);
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(cameraRotation.Pitch, cameraRotation.Yaw, 0.0f);
    }
    //
    public void UpdateMouseSensitivity()
    {
        mouseSensitivity.vertical = PlayerPrefs.GetFloat("MouseSensitivityVertical");
        mouseSensitivity.horizontal = PlayerPrefs.GetFloat("MouseSensitivityHorizontal");
        controllerSensitivity.vertical = PlayerPrefs.GetFloat("ControllerSensitivity");
        controllerSensitivity.horizontal = PlayerPrefs.GetFloat("ControllerSensitivity");
        
    }
    //
    private static int BoolToInt(bool b) => b ? 1 : -1;
}

[Serializable]
public struct MouseSensitivity
{
    public float horizontal;
    public float vertical;
    public bool invertHorizontal;
    public bool invertVertical;
}

public struct CameraRotation
{
    public float Pitch;
    public float Yaw;
}

[Serializable]
public struct CameraAngle
{
    public float min;
    public float max;
}