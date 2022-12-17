using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class EnemyAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Configure")) EnemyAIEditorWindow.Open((EnemyAI)target);
    }
}
