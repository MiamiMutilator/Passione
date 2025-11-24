using UnityEngine;

public class HitboxActivator : MonoBehaviour
{
    public Collider leftArmCollider;
    public Collider rightArmCollider;

    public void EnableLeftHitbox() => leftArmCollider.enabled = true;
    public void DisableLeftHitbox() => leftArmCollider.enabled = false;
    public void EnableRightHitbox() => rightArmCollider.enabled = true;
    public void DisableRightHitbox() => rightArmCollider.enabled = false;
}
