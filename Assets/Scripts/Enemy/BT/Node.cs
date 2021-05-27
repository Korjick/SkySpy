using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Node
{
    protected NodeState _nodeState;
    // TODO never used for continue last state if running

    public NodeState nodeState
    {
        get { return _nodeState; }
    }

    public abstract NodeState Evaluate();
}

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE,
}