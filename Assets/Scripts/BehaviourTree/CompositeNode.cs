using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [SerializeField] List<Node> _childrenNode = new List<Node>();
    public List<Node> ChildrenNode { get => _childrenNode; set => _childrenNode = value; }

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.ChildrenNode = ChildrenNode.ConvertAll(c => c.Clone());
        return node;
    }
}
