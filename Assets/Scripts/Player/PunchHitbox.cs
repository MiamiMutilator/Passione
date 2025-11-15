using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    [HideInInspector] public int baseDamage;
    [HideInInspector] public float weakpointMult;
    [HideInInspector] public LayerMask targetLayer;
    [HideInInspector] public string weakpointTag;
    [HideInInspector] public string blockedTag;
    [HideInInspector] public IAttack attack;
    [HideInInspector] public float knockbackFactor;
    
    private Vector3 baseKnockback;
    private Rage rage;
    private PlayerController player;
    private int finalDamage;
    private Vector3 finalKnockback;

    private void Awake()
    {
        rage = GetComponentInParent<Rage>();
        player = GetComponentInParent<PlayerController>();
        if (rage == null) Debug.LogError("Rage Component not found");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == null) return;

        // Get the knockback vector 
        Vector3 distanceVector = (collision.gameObject.transform.position - player.gameObject.transform.position).normalized;
        //Debug.Log("Distance vector against " + collision.gameObject.name + ": " + distanceVector);
        baseKnockback = distanceVector * knockbackFactor;
        //Debug.Log("Base knockback against " + collision.gameObject.name + ": " + baseKnockback);

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
            if (damageable.targetable && ((targetLayer & 1 << hurtbox.gameObject.layer) == 1 << hurtbox.gameObject.layer))
            {
                // If hitbox collided with block, disable hitbox, unless enraged
                if (rage && !rage.enraged && hurtbox.gameObject.CompareTag(blockedTag))
                {
                    GetComponent<Collider>().enabled = false;
                    Debug.Log($"{gameObject.name} blocked by {blockedTag}. Punch Collider: {GetComponent<Collider>().enabled}");
                    return;
                }

                // If hitbox collided with designated weakpoint (i.e Left Jab colliding with body) deal more damage and increased knockback
                if (hurtbox.gameObject.CompareTag(weakpointTag))
                {
                    finalDamage = (int)(baseDamage * weakpointMult);
                }
                else
                {
                    finalDamage = baseDamage;
                }
                
                if(damageable.TryGetComponent<EnemyHealth>(out var enemy))
                {
                    // When hitting a stunned enemy, activate time slow
                    if (enemy.isInKOState) player.ActivateTimeSlow();
                }

                finalKnockback = baseKnockback * finalDamage;
                damageable.OnHitWithKnockback(finalDamage, finalKnockback);
                attack.OnSuccessfulHit();
            }
            else
            {
                //Debug.Log(gameObject + " Targetability: " + hurtbox.targetable + " Layer: " + hurtbox.gameObject.layer);
            }
        }
        else if (collision.gameObject.TryGetComponent<IKnockback>(out var obj))
        {
            // Apply knockback

            finalKnockback = baseKnockback * baseDamage;
            obj.OnHitWithKnockback(baseDamage, finalKnockback);
        }
    }
}
