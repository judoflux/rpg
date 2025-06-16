using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AIState
{
    private Transform player;
    private NavMeshAgent agent;

    public override void Enter()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = context.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        Debug.Log("Entering Chase State");
    }

    // 其他方法保持不变...
}