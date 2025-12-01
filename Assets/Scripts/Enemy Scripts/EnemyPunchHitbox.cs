using UnityEngine;

public class EnemyPunchHitbox : MonoBehaviour
{
    public BoxCollider jab;
    public BoxCollider hook;
    public BoxCollider Cane;
    public BoxCollider Bottle;
    public bool hasBeenHit;
    public int damage;
    public bool isGrunt;
    public bool isLacrimare;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //hasBeenHit = false;
        if (isGrunt == true)
        {
            jab.enabled = false;
            hook.enabled = false;
        }
        if (isLacrimare == true)
        {
            if (Cane != null)
            {
                Cane.enabled = false;
            }
            if (Bottle != null)
            {
                Bottle.enabled = false;
            }
        }

    }

    private void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (hasBeenHit) return;
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                //Debug.Log("test");
                playerHealth.OnHit(damage);
                //hasBeenHit = true;
                if (isGrunt == true)
                {
                    jab.enabled = false;
                    hook.enabled = false;
                }
                if (isLacrimare == true)
                {
                    if (Cane != null)
                    {
                        Cane.enabled = false;
                    }
                    if (Bottle != null)
                    {
                        Bottle.enabled = false;
                    }
                }
            }

        }

        if (other.CompareTag("NearMiss"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null && player.IsDodging())
            {
                Debug.Log("Player dodged the punch");
                player.ActivateTimeSlow();
            }
        }
    }
}
