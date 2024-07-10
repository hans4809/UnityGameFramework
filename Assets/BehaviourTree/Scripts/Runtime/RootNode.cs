using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    [SerializeField] Node _childNode;
    public Node ChildNode { get => _childNode; set => _childNode = value; }
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        return ChildNode.Update();
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.ChildNode = ChildNode.Clone();
        return node;
    }
}
