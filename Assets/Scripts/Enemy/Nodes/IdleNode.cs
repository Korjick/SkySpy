using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleNode : Node
{
    private Transform[] dots;
    private NavMeshAgent agent;
    private float variation = 1;
    private Transform transform;
    private int _currentDotId;
    private Vector3 _destination;

    public IdleNode(Transform[] dots, NavMeshAgent agent, float variation, Transform transform)
    {
        this.dots = dots;
        this.agent = agent;
        this.variation = variation;
        this.transform = transform;

        _destination = dots[_currentDotId].position;
        agent.destination = _destination;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluating " + GetType());
        agent.isStopped = false;
        if (Vector3.Distance(transform.position, _destination) < variation)
        {
            _currentDotId = (_currentDotId + 1) % dots.Length;
            _destination = dots[_currentDotId].position;
            agent.destination = _destination;

            Debug.Log("Current Dot Id " + _currentDotId);
        }

        return NodeState.SUCCESS;
    }
}