using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;
using Codice.Client.Selector;
using Codice.Client.BaseCommands.BranchExplorer;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView _treeView;
    public BehaviourTreeView TreeView { get => _treeView; private set => _treeView = value; }

    BehaviourTree _tree;
    public BehaviourTree Tree { get => _tree; private set => _tree = value; }

    InspectorView _inspectorView;
    public InspectorView InspectorView { get => _inspectorView; private set => _inspectorView = value; }

    IMGUIContainer _blackboardView;
    public IMGUIContainer BlackboardView { get => _blackboardView; private set => _blackboardView = value; }
    
    ToolbarMenu _toolbarMenu;
    public ToolbarMenu ToolbarMenu { get => _toolbarMenu; private set => _toolbarMenu = value; }

    TextField _treeNameField;
    public TextField TreeNameField { get => _treeNameField; private set => _treeNameField = value; }

    TextField _locationPathField;
    public TextField LocationPathField { get => _locationPathField; private set => _locationPathField = value; }

    Button _createNewTreeButton;
    public Button CreateNewTreeButton { get => _createNewTreeButton; private set => _createNewTreeButton = value; }

    VisualElement _overlay;
    public VisualElement Overlay { get => _overlay; private set => _overlay = value; }

    BehaviourTreeSettings _settings;
    public BehaviourTreeSettings Settings { get => _settings; private set => _settings = value; }

    SerializedObject _treeObject;
    public SerializedObject TreeObject { get => _treeObject; private set => _treeObject = value; }

    SerializedProperty _blackboardProperty;
    public SerializedProperty BlackboardProperty { get => _blackboardProperty; private set => _blackboardProperty = value; }

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        wnd.minSize = new Vector2(800, 600);
    }
    
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if(Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    List<T> LoadAssets<T>() where T : UnityEngine.Object
    {
        string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        List<T> assets = new List<T>();
        foreach (string assetId in assetIds)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetId);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            assets.Add(asset);
        }
        return assets;
    }

    public void CreateGUI()
    {
        Settings = BehaviourTreeSettings.GetOrCreateSettings();

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor.uxml");
        //visualTree.CloneTree(root);
        var visualTree = Settings.BehaviourTreeXml;
        visualTree.CloneTree(root);

        //VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        //root.Add(labelFromUXML);

        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        //root.styleSheets.Add(styleSheet);
        var styleSheet = Settings.BehaviourTreeStyle;
        root.styleSheets.Add(styleSheet);

        // Main TreeView
        TreeView = root.Q<BehaviourTreeView>();
        TreeView.OnNodeSelected = OnNodeSelectedChanged;

        // Inspector View
        InspectorView = root.Q<InspectorView>();

        // Blackboard View
        BlackboardView = root.Q<IMGUIContainer>();
        BlackboardView.onGUIHandler = () =>
        {
            if(TreeObject != null && TreeObject.targetObject != null)
            {
                TreeObject.Update();
                EditorGUILayout.PropertyField(BlackboardProperty);
                TreeObject.ApplyModifiedProperties();
            }
        };

        //Toolbar asset menu;
        ToolbarMenu = root.Q<ToolbarMenu>();
        var behaviourTreeAssets = LoadAssets<BehaviourTree>();
        behaviourTreeAssets.ForEach(tree =>
        {
            ToolbarMenu.menu.AppendAction($"{tree.name}", (a) => {
                Selection.activeObject = tree;
                });
        });
        ToolbarMenu.menu.AppendSeparator();
        ToolbarMenu.menu.AppendAction("New Tree...", (a) => CreateNewTree("NewBehaviourTree"));
        
        // New Tree Dialog
        TreeNameField = root.Q<TextField>("TreeName");
        LocationPathField = root.Q<TextField>("LocationPath");
        Overlay = root.Q<VisualElement>("Overlay");
        CreateNewTreeButton = root.Q<Button>("CreateButton");
        CreateNewTreeButton.clicked += () => CreateNewTree(TreeNameField.value);
        
        if(Tree == null)
            OnSelectionChange();
        else
            SelectTree(Tree);
            

        OnSelectionChange();
    }
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

    }
    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnSelectionChange()
    {
        #region Old Code
        //BehaviourTree tree = Selection.activeObject as BehaviourTree;
        //if(!tree)
        //{
        //    if(Selection.activeGameObject)
        //    {
        //        BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
        //        if (runner != null)
        //            tree = runner.Tree;
        //    }
        //}

        //if(Application.isPlaying)
        //{
        //    if (tree)
        //        TreeView.PopulateView(tree);
        //}
        //else
        //{
        //    if (tree != null && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        //        TreeView.PopulateView(tree);
        //}

        //if(tree != null)
        //{
        //    TreeObject = new SerializedObject(tree);
        //    BlackboardProperty = TreeObject.FindProperty("_blackBoard");
        //}
        #endregion
        EditorApplication.delayCall += () =>
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (tree = null)
            {
                if(Selection.activeGameObject)
                {
                    BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if(runner != null)
                        tree = runner.Tree;
                }
            }
            SelectTree(tree);
        };
    }

    void SelectTree(BehaviourTree newTree)
    {
        if (TreeView == null)
            return;

        if (!newTree)
            return;

        this.Tree = newTree;

        Overlay.style.visibility = Visibility.Hidden;

        if (Application.isPlaying)
            TreeView.PopulateView(Tree);
        else
            TreeView.PopulateView(Tree);


        TreeObject = new SerializedObject(Tree);
        BlackboardProperty = TreeObject.FindProperty("blackboard");

        EditorApplication.delayCall += () => {
            TreeView.FrameAll();
        };
    }

    void OnNodeSelectedChanged(NodeView node)
    {
        InspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        if (TreeView != null)
            TreeView.UpdateNodeState();
    }

    void CreateNewTree(string assetName)
    {
        string path = System.IO.Path.Combine(LocationPathField.value, $"{assetName}.asset");
        BehaviourTree tree = ScriptableObject.CreateInstance<BehaviourTree>();
        tree.name = TreeNameField.ToString();
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = tree;
        EditorGUIUtility.PingObject(tree);
    }
}
