using UnityEngine;

public class EnemyPunchHitbox : MonoBehaviour
{
    public BoxCollider hitbox;
    public bool hasBeenHit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hasBeenHit = false;
    }

    // Update is called once per frame
    void Update()
    {

    }




    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenHit) return;
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnHit(3);
                hasBeenHit = true;
                hitbox.enabled = false;
            }

        }

        if (other.CompareTag("NearMiss"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null && player.IsDodging())
            {
                Debug.Log("Player dodged the punch");
                player.OnEvade();
            }
        }
    }
}
