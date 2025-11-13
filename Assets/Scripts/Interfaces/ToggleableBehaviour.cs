using UnityEngine;

public class ToggleableBehaviour : MonoBehaviour, IToggleable
{
    public virtual void Toggle(bool active)
    {
        enabled = active;
    }
}
