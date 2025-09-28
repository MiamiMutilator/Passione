using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public bool isBlockingBody;
    public bool isInKOState;
    public int health = 100;

    private Renderer rend;

    //Enemy AI
    public NavMeshAgent enemy;
    public Transform Player;
    public float distanceKeptAway = 2f;

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
        if (isInKOState == false)
        {
            float distance = Vector3.Distance(Player.position, transform.position);

            if (distance > distanceKeptAway)
            {
                enemy.SetDestination(Player.position);
            }
            else
            {
                enemy.ResetPath();
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
                health -= 1;
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
                health -= 3;
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
}
