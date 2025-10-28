using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class EnemyPathing : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public float distanceKeptAway = 2f;
    private float fightingDistance = 3f; // kept at distanceKeptAway + 1
    public float awarenessDistance = 5f;
    public float speed;
    public bool isRanged;

    [HideInInspector] public EnemyPathingState state;

    private void Start()
    {
        fightingDistance = distanceKeptAway + 1;
        agent.speed = speed;
    }

    void Update()
    {
        Pathing();
    }

    void Pathing()
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
        else if (isRanged && distance < distanceKeptAway)
        {
            Vector3 directionAway = (transform.position - player.position).normalized;

            Vector3 retreatPosition = transform.position + directionAway * (fightingDistance - distance);

            agent.SetDestination(retreatPosition);
            state = EnemyPathingState.Retreating;
        }
        else
        {
            agent.ResetPath();
            state = EnemyPathingState.Attacking;
        }
    }
}
