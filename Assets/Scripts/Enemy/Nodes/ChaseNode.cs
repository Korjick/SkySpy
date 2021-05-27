using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private Transform targetTransform;
    private NavMeshAgent agent;

    public ChaseNode(Transform targetTransform, NavMeshAgent agent)
    {
        this.targetTransform = targetTransform;
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluating " + GetType());
        agent.destination = targetTransform.position;
        return NodeState.SUCCESS;
    }
}