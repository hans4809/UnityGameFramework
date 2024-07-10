using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    BehaviourTree _tree;
    public BehaviourTree Tree { get => _tree; private set => _tree = value; }

    BehaviourTreeSettings _settings;
    public BehaviourTreeSettings Settings { get => _settings; private set => _settings = value; }

    [SerializeField] Action<NodeView> _onNodeSelected;
    public Action<NodeView> OnNodeSelected { get => _onNodeSelected; set => _onNodeSelected = value; }
    
    public struct ScriptTemplate
    {
        public TextAsset templateFile;
        public string defaultFileName;
        public string subFolder;
    }

    public ScriptTemplate[] scriptFileAssets =
    {
            new ScriptTemplate{ templateFile=BehaviourTreeSettings.GetOrCreateSettings().ScriptTemplateAcitonNode, defaultFileName="NewActionNode.cs", subFolder="Actions" },
            new ScriptTemplate{ templateFile=BehaviourTreeSettings.GetOrCreateSettings().ScriptTemplateCompositeNode, defaultFileName="NewCompositeNode.cs", subFolder="Composites" },
            new ScriptTemplate{ templateFile=BehaviourTreeSettings.GetOrCreateSettings().ScriptTemplateDecoratorNode, defaultFileName="NewDecoratorNode.cs", subFolder="Decorators" },
    };

    public BehaviourTreeView()
    {
        Settings = BehaviourTreeSettings.GetOrCreateSettings();
        
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new DoubleClickSelection());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        var styleSheet = Settings.BehaviourTreeStyle;
        if(styleSheet != null)
            styleSheets.Add(styleSheet);

        //Undo.undoRedoPerformed -= OnUndoRedo;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(Tree);
        AssetDatabase.SaveAssets();
    }

    public NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.GUID) as NodeView;
    }

    internal void PopulateView(BehaviourTree tree)
    {
        this.Tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.RootNode == null)
        {
            tree.RootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
            

        // Create NodeView
        tree.Nodes.ForEach(node => CreateNodeView(node));

        // Create Edges
        tree.Nodes.ForEach(parent =>
        {
            var children = BehaviourTree.GetChildren(parent);
            children.ForEach(child =>
            {
                NodeView parentView = FindNodeView(parent);
                NodeView childView = FindNodeView(child);

                Edge edge = parentView.Output.ConnectTo(childView.Input);
                AddElement(edge);
            }
            );
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                    Tree.DeleteNode(nodeView.Node);

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    Tree.RemoveChild(parentView.Node, childView.Node);
                }
            });

        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                Tree.AddChild(parentView.Node, childView.Node);
            });
        }

        if(graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) => {
                NodeView view = n as NodeView;
                if (view != null)
                    view.SortChildren();
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction($"Create Script.../New Action Node", (a) => CreateNewScript(scriptFileAssets[0]));
        evt.menu.AppendAction($"Create Script.../New Composite Node", (a) => CreateNewScript(scriptFileAssets[1]));
        evt.menu.AppendAction($"Create Script.../New Decorator Node", (a) => CreateNewScript(scriptFileAssets[2]));
        evt.menu.AppendSeparator();

        Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        //base.BuildContextualMenu(evt);

        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, nodePosition));
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, nodePosition));
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, nodePosition));
        }
    }

    void SelectFolder(string path)
    {
        // https://forum.unity.com/threads/selecting-a-folder-in-the-project-via-button-in-editor-window.355357/
        // Check the path has no '/' at the end, if it does remove it,
        // Obviously in this example it doesn't but it might
        // if your getting the path some other way.

        if (path[path.Length - 1] == '/')
            path = path.Substring(0, path.Length - 1);

        // Load object
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

        // Select the object in the project folder
        Selection.activeObject = obj;

        // Also flash the folder yellow to highlight it
        EditorGUIUtility.PingObject(obj);
    }
    void CreateNewScript(ScriptTemplate template)
    {
        SelectFolder($"{Settings.NewNodeBasePath}/{template.subFolder}");
        var templatePath = AssetDatabase.GetAssetPath(template.templateFile);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, template.defaultFileName);
    }

    void CreateNode(System.Type type, Vector2 position)
    {
        Node node = Tree.CreateNode(type);
        node.Position = position;
        CreateNodeView(node);
    }

    void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeState()
    {
        nodes.ForEach((n) =>
        {
            NodeView view = n as NodeView;
            if (view != null)
                view.UpdateState();
        });
    }
}
