using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    public Slider MouseSensitivitySlider;
    public Slider ControllerSensitivitySlider;
    private void OnEnable()
    {
        float sensitivityH = PlayerPrefs.GetFloat("MouseSensitivityHorizontal");
        float sensitivityV = PlayerPrefs.GetFloat("MouseSensitivityVertical");
        if (MouseSensitivitySlider != null )
        {
            MouseSensitivitySlider.SetValueWithoutNotify(sensitivityH);
            MouseSensitivitySlider.SetValueWithoutNotify(sensitivityV);
        }
        float sensitivityC = PlayerPrefs.GetFloat("ControllerSensitivity");
        if (ControllerSensitivitySlider != null)
        {
            ControllerSensitivitySlider.SetValueWithoutNotify(sensitivityC);
        }
    }
    
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
