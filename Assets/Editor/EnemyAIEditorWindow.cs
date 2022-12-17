using UnityEditor;
using UnityEngine;

public class EnemyAIEditorWindow : ExtendedEditorWindow
{
    public static void Open(EnemyAI enemyAI) {
        EnemyAIEditorWindow window = GetWindow<EnemyAIEditorWindow>("AI Module Editor");
        window.serializedObject = new SerializedObject(enemyAI);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
        DrawSideBar();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
        if (selectedProperty != null) {
            DrawProperties(selectedProperty, true);
        }
        else {
            EditorGUILayout.LabelField("Select a module");
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}
