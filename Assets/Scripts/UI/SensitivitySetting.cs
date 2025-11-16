using UnityEngine;

public class SensitivitySettings : MonoBehaviour
{
    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivityHorizontal", value);
        PlayerPrefs.SetFloat("MouseSensitivityVertical", value);
        PlayerPrefs.Save();
    }

    public void SetControllerSensitivity(float value)
    {
        PlayerPrefs.SetFloat("ControllerSensitivity", value);
        PlayerPrefs.Save();
    }
}
