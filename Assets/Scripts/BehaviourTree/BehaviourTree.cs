using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    [SerializeField] private Node _rootNode; 
    public Node RootNode { get => _rootNode; set => _rootNode = value; }

    [SerializeField] private List<Node> _nodes = new List<Node>();
    public List<Node> Nodes { get => _nodes; set => _nodes = value; }

    [SerializeField] private Node.State _treeState = Node.State.Running;
    public Node.State TreeState { get => _treeState; set => _treeState = value; }

    public Node.State Update() 
    {
        if (RootNode.CurrentState == Node.State.Running)
            TreeState = RootNode.Update();

        return TreeState;
    }

#if UNITY_EDITOR
    public Node CreateNode(System.Type type)
    {
        var node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.GUID = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        Nodes.Add(node);

        if(!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        Nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if(decorator != null)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
            decorator.ChildNode = child;
            EditorUtility.SetDirty(decorator);
        }


        RootNode rootNode = parent as RootNode;
        if (rootNode != null)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.ChildNode = child;
            EditorUtility.SetDirty(rootNode);
        }


        CompositeNode composite = parent as CompositeNode;
        if (composite != null)
        {
            Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
            composite.ChildrenNode.Add(child);
            EditorUtility.SetDirty(composite);
        }

    }

    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator != null)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
            decorator.ChildNode = null;
            EditorUtility.SetDirty(decorator);
        }


        RootNode rootNode = parent as RootNode;
        if (rootNode != null)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.ChildNode = null;
            EditorUtility.SetDirty(rootNode);
        }


        CompositeNode composite = parent as CompositeNode;
        if (composite != null)
        {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite.ChildrenNode.Remove(child);
            EditorUtility.SetDirty(composite);
        }

    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator != null && decorator.ChildNode != null)
            children.Add(decorator.ChildNode);

        RootNode rootNode = parent as RootNode;
        if (rootNode != null && rootNode.ChildNode != null)
            children.Add(rootNode.ChildNode); ;

        CompositeNode composite = parent as CompositeNode;
        if (composite != null)
            return composite.ChildrenNode;

        return children;
    }
#endif

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if(node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }
    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.RootNode = tree.RootNode.Clone();
        tree.Nodes = new List<Node>();
        Traverse(tree.RootNode, (n) =>
        {
            tree.Nodes.Add(n);
        });
        return tree;
    }
}
