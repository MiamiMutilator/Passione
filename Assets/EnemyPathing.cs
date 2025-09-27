using UnityEngine;
using UnityEngine.AI;
public class EnemyPathing : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform Player;
    public float distanceKeptAway = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
