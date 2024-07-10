using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeSettings : ScriptableObject
{
    [SerializeField] VisualTreeAsset _behaviourTreeXml;
    public VisualTreeAsset BehaviourTreeXml { get => _behaviourTreeXml; private set => _behaviourTreeXml = value; }

    [SerializeField] StyleSheet _behaviourTreeStyle;
    public StyleSheet BehaviourTreeStyle { get => _behaviourTreeStyle; private set => _behaviourTreeStyle = value; }

    [SerializeField] VisualTreeAsset _nodeXml;
    public VisualTreeAsset NodeXml { get => _nodeXml; private set => _nodeXml = value; }

    [SerializeField] TextAsset _scriptTemplateAcitonNode;
    public TextAsset ScriptTemplateAcitonNode { get => _scriptTemplateAcitonNode; private set => _scriptTemplateAcitonNode = value; }

    [SerializeField] TextAsset _scriptTemplateCompositeNode;
    public TextAsset ScriptTemplateCompositeNode { get => _scriptTemplateCompositeNode; private set => _scriptTemplateCompositeNode = value; }

    [SerializeField] TextAsset _scriptTemplateDecoratorNode;
    public TextAsset ScriptTemplateDecoratorNode { get => _scriptTemplateDecoratorNode; private set => _scriptTemplateDecoratorNode = value; }

    [SerializeField] string _newNodeBasePath = "Assets/";
    public string NewNodeBasePath { get => _newNodeBasePath; private set => _newNodeBasePath = value; }
    
    static BehaviourTreeSettings FindSettings()
    {
        var guids = AssetDatabase.FindAssets("t:BehaviourTreeSettings");
        if (guids.Length > 1)
            Debug.LogWarning($"Found multiple settingsfiles, using the first.");
    
        switch(guids.Length)
        {
            case 0:
                return null;
            default:
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(path);
        }
    }

    internal static BehaviourTreeSettings GetOrCreateSettings()
    {
        var settings = FindSettings();
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<BehaviourTreeSettings>();
            AssetDatabase.CreateAsset(settings, "Assets");
            AssetDatabase.SaveAssets();
        }
        return settings;
    }
    
    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    // Register a SettingsProvider using UIElements for the drawing framework:
    static class MyCustomSettingsUIElementsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project)
            {
                label = "BehaviourTree",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) => {
                    var settings = BehaviourTreeSettings.GetSerializedSettings();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
                    var title = new Label()
                    {
                        text = "Behaviour Tree Settings"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                    {
                        flexDirection = FlexDirection.Column
                    }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new InspectorElement(settings));

                    rootElement.Bind(settings);
                },
            };

            return provider;
        }
    }
}
