using UnityEditor;
using UnityEngine;

public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty currentProperty;

    private string selectedPropertyPath;
    protected SerializedProperty selectedProperty;

    protected void DrawProperties(SerializedProperty property, bool drawChildren) {
        string lastPropPath = string.Empty;
        EditorGUILayout.LabelField(property.displayName, EditorStyles.largeLabel);
        EditorGUILayout.Space();

        foreach (SerializedProperty prop in property) {
            if (prop.isArray && prop.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);
                EditorGUILayout.EndHorizontal();

                if (prop.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(prop, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else {
                if (!string.IsNullOrEmpty(lastPropPath) && prop.propertyPath.Contains(lastPropPath)) 
                    continue;

                lastPropPath = prop.propertyPath;
                EditorGUILayout.PropertyField(prop, drawChildren);
            }
        }

        if (GUILayout.Button("Apply")) Apply();
    }

    protected void DrawSideBar() {
        if (GUILayout.Button("Attack"))
            selectedPropertyPath = serializedObject.FindProperty("Attack").propertyPath;

        if (GUILayout.Button("Movement")) 
            selectedPropertyPath = serializedObject.FindProperty("Movement").propertyPath;

        if (!string.IsNullOrEmpty(selectedPropertyPath)) {
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }
    }

    protected void Apply() {
        serializedObject.ApplyModifiedProperties();
    }
}
