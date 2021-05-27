using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IsInAttackDistanceNode : Node
{
    private Transform targetTransform;
    private float attackRange;
    private Transform ownTransform;
    private NavMeshAgent agent;

    public IsInAttackDistanceNode(Transform targetTransform, float attackRange, Transform ownTransform,
        NavMeshAgent agent)
    {
        this.targetTransform = targetTransform;
        this.attackRange = attackRange;
        this.ownTransform = ownTransform;
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluating " + GetType());
        if (Vector3.Distance(targetTransform.position, ownTransform.position) < attackRange)
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}