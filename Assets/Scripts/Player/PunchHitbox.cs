using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public float weakpointMult;
    [HideInInspector] public LayerMask targetLayer;
    [HideInInspector] public string weakpointTag;
    [HideInInspector] public IAttack attack;
    [HideInInspector] public Vector3 knockback;

    private int finalDamage;
    private Vector3 finalKnockback;

    private void OnTriggerEnter(Collider collision)
    {
        // Check if collision is damageable
        if (collision.gameObject.TryGetComponent<DamageableCharacter>(out var hurtbox))
        {
            // If damageable, check if it is a valid target
            if (hurtbox.targetable && ((targetLayer & 1 << hurtbox.gameObject.layer) == 1 << hurtbox.gameObject.layer))
            {
                // If hitbox collided with designated weakpoint (i.e Left Jab colliding with body) deal more damage and increased knockback
                if (hurtbox.gameObject.CompareTag(weakpointTag))
                {
                    finalDamage = (int)(damage * weakpointMult);
                }
                else
                {
                    finalDamage = damage;
                }
                
                finalKnockback = knockback * finalDamage;
                hurtbox.OnHitWithKnockback(finalDamage, finalKnockback);
                attack.OnSuccessfulHit();
            }
            else
            {
                Debug.Log(gameObject + " Targetability: " + hurtbox.targetable + " Layer: " + hurtbox.gameObject.layer);
            }
        }
    }
}
