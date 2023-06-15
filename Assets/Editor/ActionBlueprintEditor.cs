using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

public class ActionBlueprintEditor : EditorWindow
{
    private ActionBlueprint currentBlueprint;
    private ActionGraphView actionGraphView;


    [MenuItem("Window/Action Tree")]
    public static void OpenActionTreeEditor()
    {
        ActionBlueprintEditor window = GetWindow<ActionBlueprintEditor>();
        window.titleContent = new GUIContent("Action Tree");
    }

    private void OnEnable()
    {
        // Check if we already have a blueprint assigned
        if (currentBlueprint == null)
        {
            string absolutePath = EditorUtility.OpenFilePanel("Select Action Blueprint",
                                                              "Assets/Resources/Battle/ActionBlueprints",
                                                              "asset");
            if (!string.IsNullOrEmpty(absolutePath))
            {
                // Convert absolute path to relative path
                string relativePath = "Assets" + absolutePath.Substring(Application.dataPath.Length);

                currentBlueprint = AssetDatabase.LoadAssetAtPath<ActionBlueprint>(relativePath);
                if (!currentBlueprint) {
                    Debug.LogWarning($"Provided asset at '{relativePath}' is not an ActionBlueprint.");
                    return;
                }

            }
        }

        // Initialize GraphView
        actionGraphView = new ActionGraphView(this, currentBlueprint);
        actionGraphView.LoadBlueprint(currentBlueprint);

        ConstructGraphView();
        ConstructUI();
    }

    private void ConstructUI()
    {
        var debugButton = new Button(DebugSelectedNode)
        {
            text = "Debug Selected Node"
        };

        rootVisualElement.Add(debugButton);
    }

    private void DebugSelectedNode()
    {
        if (actionGraphView.selection.Count == 0)
        {
            Debug.LogWarning("No node selected for debugging.");
            return;
        }

        foreach (var selectable in actionGraphView.selection)
        {
            if (selectable is ActionNodeBase node)
            {
                Debug.Log("Debug Output for Node with GUID: " + node.GUID);
               // node.DebugOutput();
            }
        }
    }

    private void ConstructGraphView()
    {
        actionGraphView = new ActionGraphView(this, currentBlueprint);
        actionGraphView.name = "Action Tree Graph";
        actionGraphView.StretchToParentSize();
        rootVisualElement.Add(actionGraphView);
    }


}