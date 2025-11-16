using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PushableObject : MonoBehaviour, IKnockback
{
    [Tooltip("Multiplied against incoming knockback")]
    [SerializeField] private float knockbackFactor = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnHitWithKnockback(int damage, Vector3 knockback)
    {
        Vector3 trueKnockback = knockback * knockbackFactor;
        Debug.Log(name + " pushed back " + trueKnockback);
        rb.AddForce(trueKnockback, ForceMode.Impulse);
    }
}
