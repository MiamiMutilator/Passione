using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RangedEnemy : EnemyHealth
{
    //Enemy AI
    public NavMeshAgent enemy;
    public Transform Player;
    public float distanceKeptAway = 2f;
    public float fightingDistance = 3f; // kept at distanceKeptAway + 1
    public float awarenessDistance = 5f;

    public override void Update()
    {
        base.Update();
        Pathing();
    }

    void Pathing()
    {
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
            else if (distance < distanceKeptAway) //for ranged enemy
            {
                Vector3 directionAway = (transform.position - Player.position).normalized;

                Vector3 retreatPosition = transform.position + directionAway * (fightingDistance - distance);

                enemy.SetDestination(retreatPosition);
            }
            else
            {
                enemy.ResetPath();
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
}
