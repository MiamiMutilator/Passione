using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public BoxCollider headBlock;
    public bool isBlockingBody;
    public BoxCollider bodyBlock;

    private int actionChosen;
    private int actionTimer;
    private bool isInAction;
    private EnemyHealth healthComponent;
    private EnemyPathing pathing;
    private Animator animator;
    private Coroutine currentAction;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<EnemyHealth>();
        pathing = GetComponent<EnemyPathing>();

        headBlock.enabled = false;
        bodyBlock.enabled = false;
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
                    actionChosen = Random.Range(0, 2);
                    StartCoroutine(ActionTaken());

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
        SetBlockingAnimations(false);
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
            //case 2: //punch
            //    Debug.Log("Punched");
            //    actionTimer = 2;
            //    yield return new WaitForSeconds(actionTimer);
            //    Debug.Log("Finished Punching");
            //    break;
            //case 3: //fake block
            //    actiontimer = 0.5f;
            //    block
            //    Debug.Log("Punched");
            //    actionTimer = 2;
            //    yield return new WaitForSeconds(actionTimer);
            //    Debug.Log("Finished Punching");
            //    break;

        }
        yield return new WaitForSeconds(0.5f);
        isInAction = false;

    }

    void SetBlockingAnimations(bool boolean)
    {
        animator.SetBool("isBlockingHead", boolean);
        animator.SetBool("isBlockingBody", boolean);
    }
}
