using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyPathing))]
public class EnemyHealth : DamageableCharacter
{
    [Tooltip("How long the enemy stays in the KO state")]
    public float koStateTime = 5f;
    [Tooltip("How long after getting hit before the enemy can be hit again")]
    public float hitCooldown = 2f;
    [Tooltip("After the enemy recovers from the KO state, their health is set to (Max Health / Health Recovery Divisor)")]
    public int healthRecoveryDivisor = 1;
    public GameObject stunnedVFX;

    public float KoCooldown = 3f;

    [HideInInspector] public bool isInKOState;
    private bool recentlyHit;
    protected Animator animator;

    public Blocking blocking;
    
    public override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        TryGetComponent<Blocking>(out blocking);
    }

    public virtual void Update()
    {
        if (depleted && !isInKOState)
        {
            StartCoroutine(KOTimer());
        }
    }

    public override void OnHit(int damage)
    {
        if (!recentlyHit)
        {
            StartCoroutine(DamageCooldown());
            base.OnHit(damage);
        }
    }

    public override void OnHitWithKnockback(int damage, Vector3 knockback)
    {
        // Enemy doesn't take knockback if they aren't in the KO state
        if (!isInKOState)
        {
            OnHit(damage);
        }
        else
        {
            //base.OnHitWithKnockback(damage, knockback);
            // For now, just destroy the enemy when hit in the KO state
            Destroy(gameObject);
        }
    }

    public virtual void RecoverHealth()
    {
        depleted = false;
        health = maxHealth / healthRecoveryDivisor;
    }

    IEnumerator KOTimer()
    {
        recentlyHit = true;
        isInKOState = true;
        stunnedVFX.SetActive(true);
        if (blocking)
        {
            animator.SetBool("isBlockingBody", false);
            animator.SetBool("isBlockingHead", false);
        }
        animator.SetBool("isWalking", false);
        animator.SetBool("isKO", true);
        yield return new WaitForSeconds(KoCooldown);
        recentlyHit = false;
        //isBlockingBody = false;
        //isBlockingHead = false;
        yield return new WaitForSeconds(koStateTime);
        RecoverHealth();
        if (blocking)
        {
            blocking.previousHealth = health;
        }
        isInKOState = false;
        animator.SetBool("isKO", false);
        stunnedVFX.SetActive(false);

    }

    IEnumerator DamageCooldown()
    {
        recentlyHit = true;
        yield return new WaitForSeconds(hitCooldown);
        recentlyHit = false;
    }
}
