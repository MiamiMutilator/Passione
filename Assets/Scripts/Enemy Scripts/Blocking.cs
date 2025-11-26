using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public BoxCollider headBlock;
    public bool isBlockingBody;
    public BoxCollider bodyBlock;
    public BoxCollider jab;
    public BoxCollider hook;
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
    int[] actionProbability = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };

    public int previousHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<EnemyHealth>();
        pathing = GetComponent<EnemyPathing>();
        enemyPunchHitbox = GetComponentInChildren<EnemyPunchHitbox>();

        headBlock.enabled = false;
        bodyBlock.enabled = false;
        jab.enabled = false;

        punchIndicator.SetActive(false);

        previousHealth = healthComponent.health;
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
                animator.SetBool("isIdle", true);
            }
            else if (pathing.state == EnemyPathingState.Chasing)
            {
                animator.SetBool("isIdle", false);
                //Debug.Log("Chasing");
                if (isBlockingBody || isBlockingHead)
                {
                    isBlockingBody = false;
                    isBlockingHead = false;
                    isInAction = false;
                    SetBlockingAnimations(false);
                }
                animator.SetBool("isWalking", pathing.state == EnemyPathingState.Chasing);
            }
            else 
            {
                animator.SetBool("isIdle", false);
                //Debug.Log("In Attack Range");
                animator.SetBool("isWalking", false);
                if (!isInAction)
                {
                    if (currentAction != null)
                    {
                        StopCoroutine(currentAction);
                    }
                    isInAction = true; 
                    actionChosen = actionProbability[Random.Range(0, actionProbability.Length)];
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BodyJab") && healthComponent.health != 0)
        {
            if (healthComponent.health < previousHealth)
            {
                StartCoroutine(TakenDamageToBody());
                previousHealth = healthComponent.health;
            }
        }

        if (other.CompareTag("HeadHook") && healthComponent.health != 0)
        {
            if (healthComponent.health < previousHealth)
            {
                StartCoroutine(TakenDamageToHead());
                previousHealth = healthComponent.health;
            }
        }

    }

    private IEnumerator ActionTaken()
    {

        isBlockingHead = false;
        isBlockingBody = false;
        SetBlockingAnimations(false);
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
                wasBlockingBody = true;
                wasBlockingHead = false;
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
                wasBlockingHead = true;
                wasBlockingBody = false;
                break;
            case 2: //jab
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Jab");
                animator.speed = 1.1f;
                //Debug.Log("Punched");
                jab.enabled = true;
                yield return StartCoroutine(WaitForAnimationToFinish("Jab"));
                animator.speed = 1f;
                //jab.enabled = false;
                //enemyPunchHitbox.hasBeenHit = false;
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
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Hook");
                animator.speed = 1.2f;
                yield return new WaitForSeconds(0.3f);
                //Debug.Log("Punched");
                hook.enabled = true;
                yield return StartCoroutine(WaitForAnimationToFinish("Hook"));
                //hook.enabled = false;
                animator.speed = 1f;
                //yield return new WaitForSeconds(0.1f);
                //enemyPunchHitbox.hasBeenHit = false;
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
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Hook");
                yield return new WaitForSeconds(0.3f);
                //Debug.Log("Punched");
                hook.enabled = true;
                yield return StartCoroutine(WaitForAnimationToFinish("Hook"));
                //hook.enabled = false;
                //yield return new WaitForSeconds(0.1f);
                //enemyPunchHitbox.hasBeenHit = false;
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
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                punchIndicator.SetActive(false);
                animator.SetTrigger("Hook");
                yield return new WaitForSeconds(0.3f);
                //Debug.Log("Punched");
                hook.enabled = true;
                yield return StartCoroutine(WaitForAnimationToFinish("Hook"));
                //hook.enabled = false;
                //yield return new WaitForSeconds(0.1f);
                //enemyPunchHitbox.hasBeenHit = false;
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
        //yield return new WaitForSeconds(0.3f);
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
        isInAction = false;
        //yield return new WaitForSeconds(0.5f);

    }

    private IEnumerator WaitForAnimationToFinish(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        float timeout = 0.6f; //if animation gets locked, exit
        float time = 0f;
        while (!stateInfo.IsName(animationName))
        {
            if (time > timeout) yield break;
            time += Time.deltaTime;

            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        while (stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f)
        {
            if (time > timeout) yield break;
            time += Time.deltaTime;

            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }

    private IEnumerator TakenDamageToHead()
    {
        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }

        isInAction = true;
        hook.enabled = false;
        jab.enabled = false;
        animator.SetBool("isBlockingBody", false);
        animator.SetTrigger("Attacked");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(WaitForAnimationToFinish("Attacked"));
        isInAction = false;
        actionChosen = Random.Range(1, 2);
        currentAction = StartCoroutine(ActionTaken());
    }

    private IEnumerator TakenDamageToBody()
    {
        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }

        isInAction = true;
        hook.enabled = false;
        jab.enabled = false;
        animator.SetBool("isBlockingHead", false);
        animator.SetTrigger("Attacked");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(WaitForAnimationToFinish("Attacked"));
        isInAction = false;
        actionChosen = Random.Range(1, 2);
        currentAction = StartCoroutine(ActionTaken());
    }


    void SetBlockingAnimations(bool boolean)
    {
        animator.SetBool("isBlockingHead", boolean);
        animator.SetBool("isBlockingBody", boolean);
    }
}
