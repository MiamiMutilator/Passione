using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Lacrimare : MonoBehaviour
{
    public GameObject punchIndicator;


    private int actionChosen;
    private int actionCooldown;
    private bool isInAction;
    private EnemyHealth healthComponent;
    private EnemyPathing pathing;
    private EnemyPunchHitbox enemyPunchHitbox;
    private Animator animator;
    private Coroutine currentAction;

    public BoxCollider Cane;
    public BoxCollider Bottle;


    bool wasBlockingHead = false;
    bool wasBlockingBody = false;
    int[] actionProbability = { 0, 1};

    void Start()
    {
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<EnemyHealth>();
        pathing = GetComponent<EnemyPathing>();
        enemyPunchHitbox = GetComponentInChildren<EnemyPunchHitbox>();

        punchIndicator.SetActive(false);


    }

    void Update()
    {
        //if (healthComponent.isInKOState) SetBlockingAnimations(false);
        //else
        //{
        //    //blocking anims
        //}

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

                //if enemy is KO'd, enemy will wipe all action states
                //if (isBlockingBody || isBlockingHead)
                //{
                //    isBlockingBody = false;
                //    isBlockingHead = false;
                //    isInAction = false;
                //    SetBlockingAnimations(false);
                //}
                animator.SetBool("isWalking", true);

            }
            else
            {
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
            //SetBlockingAnimations(false);
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
            StartCoroutine(TakenDamage());
        }
        if (other.CompareTag("HeadHook") && healthComponent.health != 0)
        {
            StartCoroutine(TakenDamage());
        }
        //DAMAGE TAKEN ANIMATIONS
        //if (other.CompareTag("BodyJab") && isBlockingHead == true && healthComponent.health != 0)
        //{
        //    StartCoroutine(TakenDamageToBody());
        //}

        //if (other.CompareTag("HeadHook") && isBlockingBody == true && healthComponent.health != 0)
        //{
        //    StartCoroutine(TakenDamageToHead());
        //}

    }

    private IEnumerator ActionTaken()
    {

        //isBlockingHead = false;
        //isBlockingBody = false;
        //SetBlockingAnimations(false);
        //yield return new WaitForSeconds(actionCooldown);
        switch (actionChosen)
        {
            case 0: // Cane Attack
                actionCooldown = Random.Range(1, 4);
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                punchIndicator.SetActive(false);
                Cane.enabled = true;
                animator.SetTrigger("Attack");
                yield return StartCoroutine(WaitForAnimationToFinish("Attack"));
                yield return new WaitForSeconds(0.1f);
                Cane.enabled = false;
                //animator set trigger Cane attack
                //hitbox enabled
                //yield return wait for animation to finish
                //return to previous animation
                break;
            case 1: // Splash Attack if too close
                actionCooldown = Random.Range(1, 4);
                punchIndicator.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                punchIndicator.SetActive(false);
                Bottle.enabled = true;
                animator.SetTrigger("Splash");
                yield return StartCoroutine(WaitForAnimationToFinish("Splash"));
                yield return new WaitForSeconds(0.1f);
                Bottle.enabled = false;
                break;
            case 2: //jab
                break;
        }
        //yield return new WaitForSeconds(0.3f);

        //go back to old blocking anim
        //if (wasBlockingHead)
        //{
        //    isBlockingHead = true;
        //    animator.SetBool("isBlockingHead", true);
        //}
        //else if (wasBlockingBody)
        //{
        //    isBlockingBody = true;
        //    animator.SetBool("isBlockingBody", true);
        //}
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

    private IEnumerator TakenDamage()
    {
        isInAction = true;
        animator.SetTrigger("Attacked");
        yield return StartCoroutine(WaitForAnimationToFinish("Attacked"));
        //yield return new WaitForSeconds(1f);
        isInAction = false;
    }



    //void SetBlockingAnimations(bool boolean)
    //{
    //    animator.SetBool("isBlockingHead", boolean);
    //    animator.SetBool("isBlockingBody", boolean);
    //}
}
