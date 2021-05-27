using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IsInChaseDistanceNode : Node
{
    private Transform targetTransform;
    private SphereCollider collider;
    private NavMeshAgent agent;
    private bool _isChasing;

    public IsInChaseDistanceNode(Transform targetTransform, SphereCollider collider, NavMeshAgent agent)
    {
        this.targetTransform = targetTransform;
        this.collider = collider;
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluating " + GetType());
        if (_isChasing)
        {
            agent.isStopped = false;
            return NodeState.SUCCESS;
        }

        if (Vector3.Distance(targetTransform.position, collider.transform.position) < collider.radius)
        {
            _isChasing = true;
            agent.isStopped = false;
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}