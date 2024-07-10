using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Runtime.Hosting;
using UnityEditor.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    [SerializeField] Node _node;
    public Node Node { get => _node; set => _node = value; }

    [SerializeField] Port _input;
    public Port Input { get => _input; set => _input = value; }

    [SerializeField] Port _output;
    public Port Output { get => _output; set => _output = value; }

    [SerializeField] Action<NodeView> _onNodeSelected;
    public Action<NodeView> OnNodeSelected { get => _onNodeSelected; set => _onNodeSelected = value; }

    public NodeView(Node node) : base ("Assets/Editor/NodeView.uxml")
    {
        this.Node = node;
        this.Node.name = node.GetType().Name;
        this.title = node.name.Replace("(Clone)", "").Replace("Node", "");
        this.viewDataKey = node.GUID;


        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInputPorts();
        CreateOutputPorts();

        SetupClasses();
        SetupDataBinding();
    }

    private void SetupDataBinding()
    {
        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(Node));
    }
    private void SetupClasses()
    {
        if (Node is ActionNode)
            AddToClassList("action");
        else if (Node is CompositeNode)
            AddToClassList("composite");
        else if (Node is DecoratorNode)
            AddToClassList("decorator");
        else if (Node is RootNode)
            AddToClassList("root");
    }

    private void CreateInputPorts()
    {
        if (Node is ActionNode)
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        else if (Node is CompositeNode)
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        else if(Node is DecoratorNode)
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));

        if (Input != null)
        {
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(Input);
        }
    }

    private void CreateOutputPorts()
    {
        if (Node is CompositeNode)
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        else if (Node is DecoratorNode)
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        else if (Node is RootNode)
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

        if (Output != null)
        {
            Output.portName = "";
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(Output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(Node, "Behaviour Tree (Set Position)");

        //Node.Position = new Vector2(newPos.xMin, newPos.yMin);
        Node._position.x = newPos.xMin;
        Node._position.y = newPos.yMin;

        EditorUtility.SetDirty(Node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null) OnNodeSelected.Invoke(this);
    }

    public void SortChildren()
    {
        CompositeNode composite = Node as CompositeNode;
        if (composite != null)
            composite.ChildrenNodes.Sort(SortByHorizontalPosition);
    }

    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.Position.x < right.Position.x ? -1 : 1;
    }

    public void UpdateState() 
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (Node.CurrentState)
            {
                case Node.State.Running:
                    if(Node.bIsStarted)
                        AddToClassList("running");
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
