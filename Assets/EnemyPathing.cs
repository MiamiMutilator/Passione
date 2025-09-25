using UnityEngine;
using UnityEngine.AI;
public class EnemyPathing : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(Player.position);
    }
}
