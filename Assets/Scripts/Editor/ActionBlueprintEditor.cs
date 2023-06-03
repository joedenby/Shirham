using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

public class ActionBlueprintEditor : EditorWindow
{
    private ActionBlueprint actionBlueprint;
    private GraphView graphView;

    [MenuItem("Window/Action Tree")]
    public static void OpenActionTreeEditor()
    {
        ActionBlueprintEditor window = GetWindow<ActionBlueprintEditor>();
        window.titleContent = new GUIContent("Action Tree");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        graphView = new ActionGraphView(null);
        graphView.name = "Action Tree Graph";
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        // Add elements to the toolbar like load/save buttons, etc.

        rootVisualElement.Add(toolbar);
    }

    // Additional methods for loading, saving, etc. go here
}