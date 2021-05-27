using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

public class AttackNode : Node
{
    private GunSystem gunSystem;
    private Transform targetTransform;
    private Transform ownTransform;

    public AttackNode(GunSystem gunSystem, Transform targetTransform, Transform ownTransform)
    {
        this.gunSystem = gunSystem;
        this.targetTransform = targetTransform;
        this.ownTransform = ownTransform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluating " + GetType());

        Vector3 lookPos = targetTransform.position - ownTransform.position;
        ownTransform.rotation = Quaternion.LookRotation(lookPos);

        if (gunSystem.BulletsLeft > 0)
        {
            gunSystem.Shoot();
        }
        else
        {
            gunSystem.Reload();
        }

        return NodeState.SUCCESS;
    }
}