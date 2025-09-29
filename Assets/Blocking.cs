using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public bool isBlockingBody;
    public bool isInKOState;
    public int health = 100;

    public int actionChosen;
    public int actionTimer;
    public bool isInAction;


    private Renderer rend;

    //Enemy AI
    public NavMeshAgent enemy;
    public Transform Player;
    public float distanceKeptAway = 2f;
    public float fightingDistance = 3f; // kept at distanceKeptAway + 1
    public float awarenessDistance = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if health == 0, KO state bool is activated and KO coroutine is activated
        if (health <= 0)
        {
            StartCoroutine(KOTimer());
        }

        if (isBlockingBody)
        {
            rend.material.color = Color.red;
        }
        if (isBlockingHead)
        {
            rend.material.color = Color.blue;
        }
        if (isInKOState)
        {
            rend.material.color = Color.yellow;
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
            else 
            {
                enemy.ResetPath();
                if (!isInAction)
                {
                    actionChosen = Random.Range(0, 2);
                    StartCoroutine(actionTaken());
                }
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BodyJab"))
        {
            if (isBlockingBody == true)
            {
                //no damage
            }
            else
            {
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.CompareTag("HeadHook"))
        {
            if (isBlockingHead)
            {
                //no damage
            }
            else
            {
                Destroy(gameObject);
            }
        }



        if (collision.gameObject.CompareTag("BodyJab") && isInKOState == true || collision.gameObject.CompareTag("HeadHook") && isInKOState == true)
        {
            //increased knockback punch done through the punch script if it collides with a KO'd enemy
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BodyJab"))
        {
            if (isBlockingBody == true)
            {
                //no damage
            }
            else
            {
                health -= 1; //switch to call to punch scripts damage
            }
        }
        if (other.gameObject.CompareTag("HeadHook"))
        {
            if (isBlockingHead)
            {
                //no damage
            }
            else
            {
                health -= 3; //switch to call to punch scripts damage
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
        isInKOState = true;
        yield return new WaitForSeconds(5);
        health = 100;
        isInKOState = false;
    }

    private IEnumerator actionTaken()
    {
        switch (actionChosen)
        {
            case 0:
                actionTimer = Random.Range(2, 7);
                isBlockingBody = true;
                isInAction = true;
                yield return new WaitForSeconds(actionTimer);
                isBlockingBody = false;
                isInAction = false;
                break;
            case 1:
                actionTimer = Random.Range(2, 7);
                isBlockingHead = true;
                isInAction = true;
                yield return new WaitForSeconds(actionTimer);
                isBlockingHead = false;
                isInAction = false;
                break;

        }
    }
}
