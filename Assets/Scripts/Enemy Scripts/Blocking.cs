using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public BoxCollider headBlock;
    public bool isBlockingBody;
    public BoxCollider bodyBlock;
    public bool isInKOState;
    public int health = 5;

    public int actionChosen;
    public int actionTimer;
    public bool isInAction;

    private bool recentlyHit = false;
    public float hitCooldown = 1f;


    private Animator animator;
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
        animator = GetComponent<Animator>();

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

        animator.SetBool("isBlockingBody", isBlockingBody);
        animator.SetBool("isBlockingHead", isBlockingHead);
        animator.SetBool("isKO", isInKOState);

        //pathing and AI
        float distance = Vector3.Distance(Player.position, transform.position);
        if (!isInKOState)
        {
            if (distance > awarenessDistance)
            {
                enemy.ResetPath();
                animator.SetBool("isWalking", false);
            }
            else if (distance > fightingDistance)
            {
                if (isBlockingBody || isBlockingHead)
                {
                    isBlockingBody = false;
                    isBlockingHead = false;
                    isInAction = false;
                    animator.SetBool("isBlockingBody", false);
                    animator.SetBool("isBlockingHead", false);
                }
                enemy.SetDestination(Player.position);
                animator.SetBool("isWalking", true);
                
            }
            //else if (distance < distanceKeptAway) //for ranged enemy
            //{
            //    Vector3 directionAway = (transform.position - Player.position).normalized;

            //    Vector3 retreatPosition = transform.position + directionAway * (fightingDistance - distance);

            //    enemy.SetDestination(retreatPosition);
            //    animator.SetBool("isWalking", true);
            //}
            else 
            {
                enemy.ResetPath();
                animator.SetBool("isWalking", false);
                if (!isInAction)
                {
                    actionChosen = Random.Range(0, 2);
                    StartCoroutine(actionTaken());

                    if (currentAction != null)
                        StopCoroutine(currentAction);

                    currentAction = StartCoroutine(actionTaken());
                }
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
            if (!bodyBlock.enabled || (playerRage.enraged))
            {
                TakeDamage(1);
            }
        }
        else if (other.gameObject.CompareTag("HeadHook"))
        {
            if (!headBlock.enabled || (playerRage.enraged))
            {
                TakeDamage(3);
            }
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
        animator.SetBool("isKO", true);
        isInKOState = true;
        isBlockingBody = false;
        isBlockingHead = false;
        yield return new WaitForSeconds(5);
        health = 5;
        isInKOState = false;
        animator.SetBool("isKO", false);
        stunnedVFX.SetActive(false);
    }

    private IEnumerator actionTaken()
    {
        isBlockingHead = false;
        isBlockingBody = false;
        isInAction = false;
        animator.SetBool("isBlockingBody", false);
        animator.SetBool("isBlockingHead", false);
        isInAction = true;
        switch (actionChosen)
        {
            case 0: // Block body
                actionTimer = Random.Range(2, 7);
                isBlockingBody = true;
                bodyBlock.enabled = true;
                animator.SetBool("isBlockingBody", true);
                yield return new WaitForSeconds(actionTimer);
                isBlockingBody = false;
                bodyBlock.enabled = false;
                animator.SetBool("isBlockingBody", false);
                break;

            case 1: // Block head
                actionTimer = Random.Range(2, 7);
                isBlockingHead = true;
                headBlock.enabled = true;
                animator.SetBool("isBlockingHead", true);
                yield return new WaitForSeconds(actionTimer);
                isBlockingHead = false;
                headBlock.enabled = false;
                animator.SetBool("isBlockingHead", false);
                break;
        }
        yield return new WaitForSeconds(0.5f);
        isInAction = false;

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
