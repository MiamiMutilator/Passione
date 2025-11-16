using UnityEngine;

public class ToggleControl : MonoBehaviour, IToggleable
{
    // Disable/Enable all toggleable components to make it easier to control input
    private ToggleableBehaviour[] components;

    private void Awake()
    {
        components = GetComponentsInParent<ToggleableBehaviour>();
    }

    private void OnEnable()
    {
        foreach (ToggleableBehaviour component in components)
        {
            component.Toggle(true);
        }
    }

    private void OnDisable()
    {
        foreach (ToggleableBehaviour component in components)
        {
            component.Toggle(false);
        }
    }

    public void Toggle(bool active) => enabled = active;
}
