using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public BoxCollider headBlock;
    public bool isBlockingBody;
    public BoxCollider bodyBlock;
    public BoxCollider punch;
    public GameObject punchIndicator;


    private int actionChosen;
    private int actionTimer;
    private bool isInAction;
    private EnemyHealth healthComponent;
    private EnemyPathing pathing;
    private EnemyPunchHitbox enemyPunchHitbox;
    private Animator animator;
    private Coroutine currentAction;

    bool wasBlockingHead = false;
    bool wasBlockingBody = false;
    int[] actionProbability = { 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 5 }; //gives probability to actions, Currently 0 = 20% chance 1 = 20% chance 3 = 30 % chance 3 and 4 = 10% chance

    void Start()
    {
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<EnemyHealth>();
        pathing = GetComponent<EnemyPathing>();
        enemyPunchHitbox = GetComponentInChildren<EnemyPunchHitbox>();

        headBlock.enabled = false;
        bodyBlock.enabled = false;
        punch.enabled = false;
        punchIndicator.SetActive(false);


    }

    void Update()
    {
        if (healthComponent.isInKOState) SetBlockingAnimations(false);
        else
        {
            animator.SetBool("isBlockingBody", isBlockingBody);
            animator.SetBool("isBlockingHead", isBlockingHead);
        }

        if (pathing == null) return;




        if (!healthComponent.isInKOState)
        {
            if (pathing.state == EnemyPathingState.Idle)
            {
                animator.SetBool("isWalking", false);
            }
            else if (pathing.state == EnemyPathingState.Chasing)
            {
                //Debug.Log("Chasing");
                if (isBlockingBody || isBlockingHead)
                {
                    isBlockingBody = false;
                    isBlockingHead = false;
                    isInAction = false;
                    SetBlockingAnimations(false);
                }
                animator.SetBool("isWalking", true);
                
            }
            else 
            {
                //Debug.Log("In Attack Range");
                animator.SetBool("isWalking", false);
                if (!isInAction)
                {
                    actionChosen = actionProbability[Random.Range(0, actionProbability.Length)];
                    if (currentAction != null)
                        StopCoroutine(currentAction);

                    currentAction = StartCoroutine(ActionTaken());
                }
            }
        }
        else
        {
            SetBlockingAnimations(false);
        }

        Vector3 direction = pathing.player.position - transform.position; //when enemy is blocking
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }
    private IEnumerator ActionTaken()
    {
        isBlockingHead = false;
        isBlockingBody = false;
        isInAction = false;
        if (isInAction) yield break;
        SetBlockingAnimations(false);
        isInAction = true;
        wasBlockingHead = isBlockingHead;
        wasBlockingBody = isBlockingBody;
        switch (actionChosen)
        {
            case 0: // Block body
                actionTimer = Random.Range(1, 2);
                isBlockingBody = true;
                bodyBlock.enabled = true;
                animator.SetBool("isBlockingBody", true);
                yield return new WaitForSeconds(actionTimer);
                isBlockingBody = false;
                bodyBlock.enabled = false;
                animator.SetBool("isBlockingBody", false);
                break;

            case 1: // Block head
                actionTimer = Random.Range(1, 2);
                isBlockingHead = true;
                headBlock.enabled = true;
                animator.SetBool("isBlockingHead", true);
                yield return new WaitForSeconds(actionTimer);
                isBlockingHead = false;
                headBlock.enabled = false;
                animator.SetBool("isBlockingHead", false);
                break;
            case 2: //jab
                enemyPunchHitbox.hasBeenHit = false;
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Jab");
                //Debug.Log("Punched");
                punch.enabled = true;
                //actionTimer = 1; //duration of punch animation
                yield return new WaitForSeconds(1);
                punch.enabled = false;
                if (wasBlockingHead)
                {
                    isBlockingHead = true;
                    animator.SetBool("isBlockingHead", true);
                }
                else if (wasBlockingBody)
                {
                    isBlockingBody = true;
                    animator.SetBool("isBlockingBody", true);
                }
                //Debug.Log("Finished Punching");
                break;
            case 3: //hook
                enemyPunchHitbox.hasBeenHit = false;
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Hook");
                //Debug.Log("Punched");
                punch.enabled = true;
                //actionTimer = 1; //duration of punch animation
                yield return new WaitForSeconds(1);
                punch.enabled = false;
                if (wasBlockingHead)
                {
                    isBlockingHead = true;
                    animator.SetBool("isBlockingHead", true);
                }
                else if (wasBlockingBody)
                {
                    isBlockingBody = true;
                    animator.SetBool("isBlockingBody", true);
                }
                //Debug.Log("Finished Punching");
                break;

            case 4: //fake block Head
                Debug.Log("Fake Blocked Head");
                isBlockingHead = true;
                headBlock.enabled = true;
                animator.SetBool("isBlockingHead", true);
                yield return new WaitForSeconds(0.5f);
                isBlockingHead = false;
                headBlock.enabled = false;
                animator.SetBool("isBlockingHead", false);
                enemyPunchHitbox.hasBeenHit = false;
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Jab");
                //Debug.Log("Punched");
                punch.enabled = true;
                //actionTimer = 1; //duration of punch animation
                yield return new WaitForSeconds(1);
                punch.enabled = false;
                if (wasBlockingHead)
                {
                    isBlockingHead = true;
                    animator.SetBool("isBlockingHead", true);
                }
                else if (wasBlockingBody)
                {
                    isBlockingBody = true;
                    animator.SetBool("isBlockingBody", true);
                }

                //Debug.Log("Finished Punching");
                break;
            case 5: //fake block body
                Debug.Log("Fake Blocked Body");
                isBlockingBody = true;
                bodyBlock.enabled = true;
                animator.SetBool("isBlockingBody", true);
                yield return new WaitForSeconds(0.5f);
                isBlockingBody = false;
                bodyBlock.enabled = false;
                animator.SetBool("isBlockingBody", false);
                enemyPunchHitbox.hasBeenHit = false;
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Jab");
                //Debug.Log("Punched");
                punch.enabled = true;
                //actionTimer = 1; //duration of punch animation
                yield return new WaitForSeconds(1);
                punch.enabled = false;
                if (wasBlockingHead)
                {
                    isBlockingHead = true;
                    animator.SetBool("isBlockingHead", true);
                }
                else if (wasBlockingBody)
                {
                    isBlockingBody = true;
                    animator.SetBool("isBlockingBody", true);
                }

                //Debug.Log("Finished Punching");
                break;


        }
        //yield return new WaitForSeconds(0.5f);
        isInAction = false;

    }


    void SetBlockingAnimations(bool boolean)
    {
        animator.SetBool("isBlockingHead", boolean);
        animator.SetBool("isBlockingBody", boolean);
    }
}
