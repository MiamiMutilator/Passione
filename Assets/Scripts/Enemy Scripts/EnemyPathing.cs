using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class EnemyPathing : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public float distanceKeptAway = 2f;
    public float fightingDistance = 3f;
    public float awarenessDistance = 15f;
    public float speed = 5f;
    public bool isRanged;

    public EnemyHealth enemyHealth;

    [HideInInspector] public EnemyPathingState state;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        agent.speed = speed;

        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }
    }

    void Update()
    {
        //Debug.Log(state.ToString());
        if (!enemyHealth.isInKOState)
        {
            Pathing();
        }
    }

    public void Pathing()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > awarenessDistance)
        {
            agent.ResetPath();
            state = EnemyPathingState.Idle;
        }
        else if (distance > fightingDistance)
        {
            agent.SetDestination(player.position);
            state = EnemyPathingState.Chasing;

        }
        else if (isRanged && distance <= distanceKeptAway)
        {
            Vector3 directionAway = (transform.position - player.position).normalized;

            Vector3 retreatPosition = transform.position + directionAway * (fightingDistance - distance);

            agent.SetDestination(retreatPosition);
            state = EnemyPathingState.Retreating;
        }
        else if (distance <= fightingDistance)
        {
            agent.ResetPath();
            state = EnemyPathingState.Attacking;
        }
    }
}
