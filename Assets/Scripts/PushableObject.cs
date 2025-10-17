using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PushableObject : MonoBehaviour, IKnockback
{
    [Tooltip("Multiplied against incoming damage and knockback")]
    [SerializeField] private float knockbackFactor = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnHitWithKnockback(int damage, Vector3 knockback)
    {
        rb.AddForce(damage * knockbackFactor * knockback, ForceMode.Impulse);
    }
}
