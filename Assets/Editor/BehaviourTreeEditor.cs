using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView _treeView;
    public BehaviourTreeView TreeView { get => _treeView; private set => _treeView = value; }

    InspectorView _inspectorView;
    public InspectorView InspectorView { get => _inspectorView; private set => _inspectorView = value; }

    IMGUIContainer _blackboardView;
    public IMGUIContainer BlackboardView { get => _blackboardView; private set => _blackboardView = value; }

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

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        //VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        //root.Add(labelFromUXML);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        TreeView = root.Q<BehaviourTreeView>();
        InspectorView = root.Q<InspectorView>();
        BlackboardView = root.Q<IMGUIContainer>();
        BlackboardView.onGUIHandler = () =>
        {
            TreeObject.Update();
            EditorGUILayout.PropertyField(BlackboardProperty);
            TreeObject.ApplyModifiedProperties();
        };

        TreeView.OnNodeSelected = OnNodeSelectedChanged;

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
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if(!tree)
        {
            if(Selection.activeGameObject)
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner != null)
                    tree = runner.Tree;
            }
        }

        if(Application.isPlaying)
        {
            if (tree)
                TreeView.PopulateView(tree);
        }
        else
        {
            if (tree != null && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                TreeView.PopulateView(tree);
        }

        if(tree != null)
        {
            TreeObject = new SerializedObject(tree);
            BlackboardProperty = TreeObject.FindProperty("_blackBoard");
        }
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
}
