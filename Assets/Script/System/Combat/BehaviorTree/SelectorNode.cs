using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    public SelectorNode() : base() {}

    public SelectorNode(List<Node> children) : base(children) {}

    public override ENodeState Evaluate()
    {
        foreach(Node node in childrenNode)
        {
            switch(node.Evaluate())
            {
                case ENodeState.Failed:
                continue;
                case ENodeState.Success:
                return state = ENodeState.Success;
                case ENodeState.Running:
                return state = ENodeState.Running;
                default:
                continue;
            }
        }

        return state = ENodeState.Failed;
    }
}
