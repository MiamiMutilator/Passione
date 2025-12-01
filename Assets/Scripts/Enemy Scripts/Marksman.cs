using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyPathing))]
public class Marksman : EnemyHealth
{
    /*
     * "Marksmen engage in ranged attacks on the player. They have low health, move quicker than the player, and do moderate damage. Marksmen attempt to maintain a wide distance from the player at all times."

    The idea is that they constantly try to have some distance on the player to attack them from, running away when the player gets too close, until they're cornered or otherwise can't quite go anywhere else. 

    They fire a 6 shot revolver that's a hit-scan in a straight line with an effective range of at most 30 units, 
    so it would make sense to give them chance percentages on whether their shots will land or not based on if they're closer or farther from the player / that 30 unit maximum. 
    One shot should do 1 point of damage, but this of course can be made into a public variable to be adjustable.

    Animations for standing and shooting, running, and reloading after firing 6 times would be relevant.
     */

    public int damage = 1;
    public float rotationSpeed = 5f;
    public int maxAmmo = 6;
    public float shotRange = 30f;
    [Tooltip("The time before a shot is fired. Used to give visual cues for an incoming shot")]
    public float aimingTime = 0.6f;
    [Tooltip("The time between shots")]
    public float shotCooldown = 1f;
    [Tooltip("How long it takes for the enemy to reload.")]
    public float reloadTime = 2f;
    [Tooltip("The minimum distance before the enemy can start to miss the player with its shots.")]
    public float accuracyFalloffThreshold = 7f;
    [Tooltip("Accuracy of a shot = 100 - (max[0, distance from player - falloff threshold] * falloff factor)")]
    public float accuracyFalloffFactor = 4f;
    public Transform firePoint;
    public Color firelineAimColor = Color.yellow;
    public Color firelineShootColor = Color.red;

    private EnemyPathing pathing;
    private int currentAmmo;
    private bool isFiring;
    private bool isAiming;
    private LayerMask targetLayer;
    private bool isReloading;
    private bool shotTriggered;
    private readonly string[] animations = new string[3] { "isIdle", "isWalking", "isAiming" };
    private bool stunned = false;
    private Coroutine aimRoutine;
    private Coroutine reloadRoutine;
    private Coroutine shotRoutine;
    LineRenderer lineRenderer;

    public override void Start()
    {
        base.Start(); // Gets the Animator and Rigidbody

        pathing = GetComponent<EnemyPathing>();
        TryGetComponent<LineRenderer>(out lineRenderer);
        currentAmmo = maxAmmo;
        targetLayer = LayerMask.GetMask("Player"); // only hits the Player layer
    }

    public override void Update()
    {
        base.Update(); // Handles KO state

        if (isInKOState)
        {
            pathing.agent.isStopped = true;
            ToggleAnimation("None");
            StopAllActionCoroutines();
            return;
        }
        else
        {
            pathing.agent.isStopped = false;
        }

        if (isAiming && lineRenderer != null) DrawLineToPlayer(firelineAimColor);
        else if (shotTriggered && lineRenderer != null) DrawLineToPlayer(firelineShootColor);
        else lineRenderer.enabled = false;

        if (shotTriggered || stunned || pathing == null || isReloading || isAiming) return;

        if (currentAmmo <= 0) reloadRoutine = StartCoroutine(Reload());
        else if (pathing.state == EnemyPathingState.Attacking)
        {
            if (currentAmmo > 0 && !isFiring)
            {
                aimRoutine = StartCoroutine(Aim());
            }
        }

        if (animator != null) Animate();
    }

    public override void OnHit(int damage)
    {
        if (!recentlyHit)
        {
            recentlyHit = true;
            StartCoroutine(DamageCooldown());
            StartCoroutine(Attacked());
            Health -= damage;

            if (Health <= 0) stunnedVFX.transform.position = transform.position + new Vector3(0, 2.4f);

            Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
        }
    }

    IEnumerator Attacked()
    {
        if (stunned) yield break;

        stunned = true;

        // Stop all actions
        isAiming = false;
        isFiring = false;
        isReloading = false;

        StopAllActionCoroutines();

        // Force all animations off
        ToggleAnimation("None");

        // Trigger attacked
        animator.ResetTrigger("Shoot"); // prevent competing trigger
        animator.SetTrigger("Attacked");

        yield return WaitForAnimationToFinish("Attacked");

        animator.ResetTrigger("Attacked");
        stunned = false;
        shotTriggered = false;
    }

    void Animate()
    {
        if (shotTriggered || stunned) return;

        switch (pathing.state)
        {
            case EnemyPathingState.Attacking:
                FacePlayer();
                if (isAiming) ToggleAnimation("isAiming");
                else ToggleAnimation("None");
                
                break;
            case EnemyPathingState.Chasing:
            case EnemyPathingState.Retreating:
                ToggleAnimation("isWalking");
                break;
            default:
                ToggleAnimation("isIdle");
                break;
        }
    }

    private void ToggleAnimation(string name)
    {
        foreach (string anim in animations)
        {
            animator.SetBool(anim, anim.Equals(name));
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = pathing.player.position - transform.position;
        direction.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRot);

        if (angle > 140f)
        {
            // Rotate faster to prevent awkward slow rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime * 100
            );
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    IEnumerator Aim()
    {
        isAiming = true;
        Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.white, aimingTime);

        yield return new WaitForSeconds(aimingTime);

        isAiming = false;
        Shoot();
    }

    void DrawLineToPlayer(Color color)
    {
        if (lineRenderer == null || firePoint == null || pathing.player == null)
            return;

        lineRenderer.enabled = true;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, pathing.player.position);
    }

    void Shoot()
    {
        lineRenderer.startColor = firelineShootColor;
        lineRenderer.endColor = firelineShootColor;

        animator.SetTrigger("Shoot");
        ToggleAnimation("None");
        shotTriggered = true;

        shotRoutine = StartCoroutine(ShotCooldown());
        currentAmmo--;

        // Cast a ray toward the player's position. If it hits an object with the Player layer, deal damage using the IDamageable component
        if (Physics.Raycast(firePoint.position, pathing.player.position - firePoint.position, out RaycastHit hit, shotRange, targetLayer))
        {
            if (hit.collider.gameObject == null) return;

            PlayerController player = hit.collider.gameObject.GetComponentInParent<PlayerController>();
            if (player != null && player.IsDodging())
            {
                Debug.Log("Player dodged the bullet");
                player.ActivateTimeSlow();
                return;
            }

            // Calculate accuracy reduction based on distance from the player
            float accuracy = 100 - (Mathf.Max(0, Vector3.Distance(pathing.player.position, transform.position) - accuracyFalloffThreshold) * accuracyFalloffFactor);
            float chance = Random.Range(0, 100f);

            if (accuracy != 0 && chance <= accuracy)
            {
                // Shot landed

                var damageable = hit.collider.gameObject.GetComponentInParent<IDamageable>();
                if (damageable != null && !stunned)
                {
                    Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.green, 0.5f);
                    //Debug.Log($"Shot successfully hit {hit.collider.gameObject.name} with {accuracy}% chance to hit and a roll of {chance}");
                    damageable.OnHit(damage);
                }
                else if (damageable == null)
                {
                    Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.magenta, 0.5f);
                    Debug.LogError("IDamageable not found on gameObject " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.yellow, 0.5f);
                //Debug.Log($"Shot missed with {accuracy}% chance to hit and a roll of {chance}");
            }
        }
        else
        {
            //if (hit.collider.gameObject != null) Debug.Log("Wrong target. Shot hit " + hit.collider.gameObject.name);
            Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.red, 0.5f);
        }

        
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    IEnumerator ShotCooldown()
    {
        isFiring = true;

        yield return WaitForAnimationToFinish("Shoot");

        shotTriggered = false;
        animator.ResetTrigger("Shoot");
        
        yield return new WaitForSeconds(shotCooldown);
        isFiring = false;
    }
    private void StopAllActionCoroutines()
    {
        if (aimRoutine != null) StopCoroutine(aimRoutine);
        if (reloadRoutine != null) StopCoroutine(reloadRoutine);
        if (shotRoutine != null) StopCoroutine(shotRoutine);

        aimRoutine = null;
        reloadRoutine = null;
        shotRoutine = null;
    }

    private IEnumerator WaitForAnimationToFinish(string animationName, float timeout = 2f)
    {
        float timer = 0f;

        // Wait for the animation to start playing
        while (true)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName(animationName))
                break; // We are in the animation, continue below

            if (timer > timeout)
                yield break; // Give up after timeout

            timer += Time.deltaTime;
            yield return null;
        }

        // Wait for the animation to finish playing
        while (true)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);

            // If finished
            if (!state.IsName(animationName) || state.normalizedTime >= 1f)
                break;

            if (timer > timeout)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
