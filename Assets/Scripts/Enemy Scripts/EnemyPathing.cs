using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class EnemyPathing : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public float distanceKeptAway = 2f;
    public float fightingDistance = 10f;
    public float awarenessDistance = 10f;
    public float speed;
    public bool isRanged;

    [HideInInspector] public EnemyPathingState state;

    private void Start()
    {
        agent.speed = speed;
    }

    void Update()
    {
        //Debug.Log(state.ToString());
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
        else if (isRanged && distance < fightingDistance)
        {
            agent.ResetPath();
            state = EnemyPathingState.Attacking;
        }
        else
        {
            agent.ResetPath();
        }
    }
}
