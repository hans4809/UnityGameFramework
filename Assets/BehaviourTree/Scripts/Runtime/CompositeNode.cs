using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [SerializeField] List<Node> _childrenNodes = new List<Node>();
    public List<Node> ChildrenNodes { get => _childrenNodes; set => _childrenNodes = value; }

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.ChildrenNodes = ChildrenNodes.ConvertAll(c => c.Clone());
        return node;
    }
}
