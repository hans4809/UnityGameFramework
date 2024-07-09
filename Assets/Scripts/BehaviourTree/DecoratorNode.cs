using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorNode : Node
{
    [SerializeField] private Node _childNode;
    public Node ChildNode { get => _childNode; set => _childNode = value; }

    public override Node Clone()
    {
        DecoratorNode node = Instantiate(this);
        node.ChildNode = ChildNode.Clone();
        return node;
    }
}

