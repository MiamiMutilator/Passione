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

    public int damage = 2;
    public int maxAmmo = 6;
    public float shotRange = 30f;
    public float shotCooldown = 1f;
    public Transform firePoint;

    private EnemyPathing pathing;
    private int currentAmmo;
    private bool currentlyFiring;
    private LayerMask targetLayer;

    public override void Start()
    {
        base.Start();
        pathing = GetComponent<EnemyPathing>();
        currentAmmo = maxAmmo;
        targetLayer = ~LayerMask.GetMask("Player"); // ignore all layers except for Player
    }

    public override void Update()
    {
        base.Update();

        if (pathing == null) return;
        
        if (pathing.state == EnemyPathingState.Attacking)
        {
            if (currentAmmo > 0 && !currentlyFiring)
            {
                StartCoroutine(ShotCooldown());
                Shoot();
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;

        if (Physics.Raycast(firePoint.position, pathing.player.position - firePoint.position, out RaycastHit hit, shotRange, targetLayer))
        {
            if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.OnHit(null, damage);
            }
        }
    }

    IEnumerator ShotCooldown()
    {
        currentlyFiring = true;
        yield return new WaitForSeconds(shotCooldown);
        currentlyFiring = false;
    }
}
