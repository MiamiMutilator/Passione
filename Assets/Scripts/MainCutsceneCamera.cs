using UnityEngine;

public class CutsceneCameraLock : MonoBehaviour
{
    void Start()
    {
        GameObject camObj = GameObject.Find("MainCutsceneCamera");

        if (camObj == null)
        {
            Debug.LogError("MainCutsceneCamera not found in scene!");
            return;
        }

        MonoBehaviour[] scripts = camObj.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            // Don't disable the Camera component itself
            if (!(script is Camera))
                script.enabled = false;
        }
    }
}
