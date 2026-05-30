using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : Node
{
    public SequenceNode() : base() {}

    public SequenceNode(List<Node> children) : base(children) {}

    public override ENodeState Evaluate()
    {
        bool bNowRunning  = false;

        foreach(Node node in childrenNode)
        {
            switch(node.Evaluate())
            {
                case ENodeState.Failed:
                return state = ENodeState.Failed;
                case ENodeState.Running:
                    bNowRunning = true;
                continue;
                case ENodeState.Success:
                continue;
                default:
                continue;
            }
        }

        return state = bNowRunning ? ENodeState.Running : ENodeState.Success;
    }
}
