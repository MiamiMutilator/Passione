using UnityEngine;
using System.Collections;

public class CameraDollyEffect : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform;       // Main camera
    public Transform targetLookAt;          // Dolly focus target
    public Vector3 targetPositionOffset;    // Offset from target
    public float moveDuration = 2f;         // Dolly duration
    public float destroyDelay = 5f;         // Optional delay before destroying script

    private PlayerController playerController;
    private Transform originalCameraParent;
    private GameObject dollyRig;
    private bool dollyStarted = false;

    private MonoBehaviour[] cameraControllers;

    private void Update() { }   // Do nothing until trigger
    private void LateUpdate() { }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || dollyStarted)
            return;

        dollyStarted = true;

        // Freeze player input
        playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        // Disable camera controllers before parenting
        cameraControllers = cameraTransform.GetComponents<MonoBehaviour>();
        foreach (var script in cameraControllers)
            script.enabled = false;

        // Save original parent
        originalCameraParent = cameraTransform.parent;

        // Create dolly rig at camera's current world position and rotation
        dollyRig = new GameObject("DollyRig");
        dollyRig.transform.position = cameraTransform.position;
        dollyRig.transform.rotation = cameraTransform.rotation;

        // Parent camera to dolly rig and reset local position/rotation to zero
        cameraTransform.SetParent(dollyRig.transform);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;

        // Start the dolly coroutine
        StartCoroutine(PerformCameraDolly());
    }

    private IEnumerator PerformCameraDolly()
    {
        Vector3 startPos = dollyRig.transform.position;
        Quaternion startRot = dollyRig.transform.rotation;

        Vector3 targetPos = targetLookAt.position + targetPositionOffset;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / moveDuration));

            // Move dolly rig toward target position smoothly
            dollyRig.transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Rotate dolly rig to always look at the target
            Vector3 lookDirection = targetLookAt.position - dollyRig.transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                dollyRig.transform.rotation = Quaternion.Slerp(
                    startRot,
                    Quaternion.LookRotation(lookDirection),
                    t
                );
            }

            yield return null;
        }

        // Ensure final position and rotation
        dollyRig.transform.position = targetPos;
        dollyRig.transform.rotation = Quaternion.LookRotation(targetLookAt.position - dollyRig.transform.position);

        // Re-parent camera to original parent
        cameraTransform.SetParent(originalCameraParent);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;

        // Re-enable camera scripts
        foreach (var script in cameraControllers)
            script.enabled = true;

        // Unfreeze player input
        if (playerController != null)
            playerController.enabled = true;

        // Destroy temporary dolly rig
        Destroy(dollyRig);

        // Optional: destroy this script after a delay
        yield return new WaitForSeconds(destroyDelay);
        Destroy(this);
    }
}
