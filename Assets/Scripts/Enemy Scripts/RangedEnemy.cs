using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    public bool isInKOState;
    public int health = 5;

    public int actionTimer;
    public bool isInAction;

    private bool recentlyHit = false;
    public float hitCooldown = 1f;


    private Coroutine currentAction;

    //vfx
    public GameObject stunnedVFX;

    //Enemy AI
    public NavMeshAgent enemy;
    public Transform Player;
    public float distanceKeptAway = 2f;
    public float fightingDistance = 3f; // kept at distanceKeptAway + 1
    public float awarenessDistance = 5f;

    //rage
    public Rage playerRage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRage = FindObjectOfType<Rage>();
    }

    // Update is called once per frame
    void Update()
    {
        //if health == 0, KO state bool is activated and KO coroutine is activated
        if (health <= 0 && !isInKOState)
        {
            StartCoroutine(KOTimer());
        }

        //pathing and AI
        float distance = Vector3.Distance(Player.position, transform.position);
        if (!isInKOState)
        {
            if (distance > awarenessDistance)
            {
                enemy.ResetPath();
            }
            else if (distance > fightingDistance)
            {
                enemy.SetDestination(Player.position);

            }
            else if (distance < distanceKeptAway) //for ranged enemy
            {
                Vector3 directionAway = (transform.position - Player.position).normalized;

                Vector3 retreatPosition = transform.position + directionAway * (fightingDistance - distance);

                enemy.SetDestination(retreatPosition);
            }
            else
            {
                enemy.ResetPath();
            }
        }
        Vector3 direction = Player.position - transform.position; //when enemy is blocking
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (recentlyHit) return;

        if (other.gameObject.CompareTag("BodyJab"))
        {
            TakeDamage(1);
        }
        else if (other.gameObject.CompareTag("HeadHook"))
        {
            TakeDamage(3);
        }
        if (other.gameObject.CompareTag("BodyJab") && isInKOState == true || other.gameObject.CompareTag("HeadHook") && isInKOState == true)
        {
            Destroy(gameObject);
            //increased knockback punch done through the punch script if it collides with a KO'd enemy
        }

    }

    private IEnumerator KOTimer()
    {
        stunnedVFX.SetActive(true);
        isInKOState = true;
        yield return new WaitForSeconds(5);
        health = 5;
        isInKOState = false;
        stunnedVFX.SetActive(false);
    }


    private IEnumerator DamageCooldown()
    {
        recentlyHit = true;
        yield return new WaitForSeconds(hitCooldown);
        recentlyHit = false;
    }

    private void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(DamageCooldown());
    }
}
