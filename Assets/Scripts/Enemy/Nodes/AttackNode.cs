using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Player;
using UnityEngine;

public class AttackNode : Node
{
    private GunSystem gunSystem;

    public AttackNode(GunSystem gunSystem)
    {
        this.gunSystem = gunSystem;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluating " + GetType());

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