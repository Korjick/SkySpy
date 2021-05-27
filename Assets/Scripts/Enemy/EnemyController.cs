using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform[] dots;
    public Transform player;
    public SphereCollider sphereCollider;
    public float destinationVariation = 5;
    public float attackRange = 3;

    private NavMeshAgent _agent;
    private GunSystem _gunSystem;
    private Node _topNode;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _gunSystem = GetComponent<GunSystem>();
    }

    private void Start()
    {
        ConstructBehaviourTree();
    }

    private void ConstructBehaviourTree()
    {
        Transform ownTransform = transform;

        IdleNode idleNode = new IdleNode(dots, _agent, destinationVariation, ownTransform);

        ChaseNode chaseNode = new ChaseNode(player, _agent);
        IsInChaseDistanceNode isInChaseDistanceNode = new IsInChaseDistanceNode(player, sphereCollider, _agent);

        AttackNode attackNode = new AttackNode(_gunSystem);
        IsInAttackDistanceNode isInAttackDistanceNode =
            new IsInAttackDistanceNode(player, attackRange, ownTransform, _agent);

        Sequence chaseSequence = new Sequence(new List<Node> {isInChaseDistanceNode, chaseNode});
        Sequence attackSequence = new Sequence(new List<Node> {isInAttackDistanceNode, attackNode});

        Selector mainSelector = new Selector(new List<Node> {attackSequence, chaseSequence, idleNode});

        _topNode = mainSelector;
    }

    private void Update()
    {
        _topNode.Evaluate();
        if (_topNode.nodeState == NodeState.FAILURE)
        {
            _agent.isStopped = true;
        }
    }
}