using System.Collections;
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

    private EnemyPathing pathing;
    private int currentAmmo;
    private bool currentlyFiring;
    private bool currentlyAiming;
    private LayerMask targetLayer;
    private bool isReloading;

    private Renderer appearance; // Testing
    private Color baseColor; // Testing

    public override void Start()
    {
        base.Start(); // Gets the Animator and Rigidbody

        pathing = GetComponent<EnemyPathing>();
        currentAmmo = maxAmmo;
        targetLayer = LayerMask.GetMask("Player"); // only hits the Player layer
        appearance = GetComponent<Renderer>();
        baseColor = appearance.material.color;
    }

    public override void Update()
    {
        base.Update(); // Handles KO state

        if (pathing == null || isReloading || currentlyAiming) return;

        if (currentAmmo <= 0) StartCoroutine(Reload());
        else if (pathing.state == EnemyPathingState.Attacking)
        {
            if (currentAmmo > 0 && !currentlyFiring)
            {
                StartCoroutine(Aim());
            }
        }
    }

    IEnumerator Aim()
    {
        appearance.material.SetColor("_BaseColor", Color.red); // Testing
        currentlyAiming = true;
        Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.white, aimingTime);

        yield return new WaitForSeconds(aimingTime);

        appearance.material.SetColor("_BaseColor", baseColor); // Testing
        currentlyAiming = false;
        Shoot();
    }

    void Shoot()
    {
        StartCoroutine(ShotCooldown());
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
                if (damageable != null)
                {
                    Debug.DrawRay(firePoint.position, pathing.player.position - firePoint.position, Color.green, 0.5f);
                    //Debug.Log($"Shot successfully hit {hit.collider.gameObject.name} with {accuracy}% chance to hit and a roll of {chance}");
                    damageable.OnHit(damage);
                }
                else
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
        currentlyFiring = true;
        yield return new WaitForSeconds(shotCooldown);
        currentlyFiring = false;
    }
}
