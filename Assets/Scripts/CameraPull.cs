using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CameraPull : MonoBehaviour
{
    [Header("Camera + Player")]
    public GameObject Camera;
    public PlayerInput playerInput;
    public GameObject player;
    public Transform cameraTransform;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;      // Affects timing of movement
    public float rotateSpeed = 150f;  // Used only for tweakable feel

    [Header("Cinematic Motion Curves")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve rotateCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool sequenceRunning = false;

    void OnTriggerEnter(Collider other)
    {
        if (!sequenceRunning && other.CompareTag("Player"))
        {
            Transform root = other.transform.root;

            if (root.CompareTag("Player"))
            {
                StartCoroutine(CameraSequence());
            }
        }
    }

    IEnumerator CameraSequence()
    {
        sequenceRunning = true;

        // Enable cinematic camera
        Camera.SetActive(true);

        // Disable player controls
        playerInput.actions.FindActionMap("Player").Disable();
        player.SetActive(false);

        // Cache directions before movement begins
        Vector3 forward1 = cameraTransform.forward;       // Step 1 forward
        Vector3 right = cameraTransform.right;         // Step 2 side movement
        Quaternion startRot = cameraTransform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, -180f, 0);

        // Step 1: Smooth forward move
        yield return MoveForDistance(forward1, 5f);

        // Step 2: Smooth rightward curve + rotation together
        yield return MoveSideAndRotate(right, endRot, 7f);

        // Step 3: Smooth forward again using new orientation
        Vector3 forward2 = cameraTransform.forward;
        yield return MoveForDistance(forward2, 5f);

        // Pause at the final location
        yield return new WaitForSeconds(2f);

        // Restore gameplay
        Camera.SetActive(false);
        playerInput.actions.FindActionMap("Player").Enable();
        player.SetActive(true);

        sequenceRunning = false;
        Destroy(this);
    }

    // ------------------------------
    // CINEMATIC SMOOTH MOVEMENT
    // ------------------------------

    IEnumerator MoveForDistance(Vector3 direction, float distance)
    {
        float time = 0f;
        float duration = distance / moveSpeed;

        Vector3 startPos = cameraTransform.position;
        Vector3 endPos = startPos + direction * distance;

        while (time < duration)
        {
            float t = time / duration;
            float eased = moveCurve.Evaluate(t);

            cameraTransform.position = Vector3.Lerp(startPos, endPos, eased);

            time += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = endPos;
    }

    IEnumerator MoveSideAndRotate(Vector3 sideDirection, Quaternion endRot, float distance)
    {
        float time = 0f;
        float duration = distance / moveSpeed;

        Vector3 startPos = cameraTransform.position;
        Vector3 endPos = startPos + sideDirection * distance;

        Quaternion startRot = cameraTransform.rotation;

        while (time < duration)
        {
            float t = time / duration;

            float moveT = moveCurve.Evaluate(t);
            float rotateT = rotateCurve.Evaluate(t);

            // Smooth curved movement
            cameraTransform.position = Vector3.Lerp(startPos, endPos, moveT);

            // Smooth curved rotation
            cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, rotateT);

            time += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = endPos;
        cameraTransform.rotation = endRot;
    }
}
