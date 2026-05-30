using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ENodeState
{
    Running, // InProgress
    Failed,
    Success,
    //InProgress,
}

public abstract class Node
{
    protected ENodeState state;
    public Node parentNode;
    protected List<Node> childrenNode = new List<Node>();

    public Node()
    {
        parentNode = null;
    }

    public Node(List<Node> children)
    {
        foreach(var child in children)
        {
            AttachChild(child);
        }
    }

    public void AttachChild(Node child)
    {
        childrenNode.Add(child);
        child.parentNode = this;
    }

    public abstract ENodeState Evaluate();
}
