using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class LaunchableObject : MonoBehaviour, IKnockback
{
    [Tooltip("Multiplied against incoming knockback")]
    [SerializeField] private float knockbackFactor = 1f;
    [Tooltip("Base damage is multiplied against the magnitude of the object's linear velocity vector while in motion. Deals damage to DamageableCharacters")]
    [SerializeField] private int baseDamage = 1;

    private Rigidbody rb;
    private Vector3 knockback;
    private int finalDamage;

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

    // Deal damage when moving
    private void OnTriggerEnter(Collider collision)
    {
        if (rb.linearVelocity == Vector3.zero || collision.gameObject == null) return;

        // Get the knockback vector 
        Vector3 distanceVector = (collision.gameObject.transform.position - gameObject.transform.position).normalized;
        knockback = distanceVector;

        // Check if collision is damageable
        if (collision.gameObject.TryGetComponent<Hurtbox>(out var hurtbox))
        {
            DamageableCharacter damageable = hurtbox.damageableCharacter;
            if (damageable == null)
            {
                Debug.LogError("Hurtbox has no DamageableCharacter component assigned");
                return;
            }

            // If damageable, check if it is a valid target
            if (damageable.targetable)
            {
                finalDamage = baseDamage * (int)(rb.linearVelocity.magnitude);

                Vector3 finalKnockback = knockback * finalDamage;
                damageable.OnHitWithKnockback(finalDamage, finalKnockback);
            }
            else
            {
                //Debug.Log(gameObject + " Targetability: " + hurtbox.targetable + " Layer: " + hurtbox.gameObject.layer);
            }
        }
        else if (collision.gameObject.TryGetComponent<IKnockback>(out var obj))
        {
            // Apply knockback

            Vector3 finalKnockback = knockback * rb.linearVelocity.magnitude;
            obj.OnHitWithKnockback(baseDamage, finalKnockback);
        }
    }
}
