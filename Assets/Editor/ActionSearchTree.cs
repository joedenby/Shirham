using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

public class ActionSearchTree : ScriptableObject, ISearchWindowProvider
{
    private ActionGraphView actionGraphView;
    private EditorWindow editorWindow; 
    private Texture2D indentIcon;

    public void Init(EditorWindow editorWindow, ActionGraphView actionGraphView)
    {
        this.actionGraphView = actionGraphView;
        this.editorWindow = editorWindow;

        // Transparent 1x1 texture used for indentation
        indentIcon = new Texture2D(1, 1);
        indentIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        indentIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 0)
        };


        // Get all types derived from ActionNode
        var derivedTypes = GetDerivedTypesOfActionNode();
        derivedTypes.OrderByDescending(evt => 
            (string)evt.GetProperty("Category", BindingFlags.Instance | BindingFlags.Public).GetValue(Activator.CreateInstance(evt)));

        string prev = string.Empty;
        foreach (Type derivedType in derivedTypes)
        {
            var categoryProperty = derivedType.GetProperty("Category", BindingFlags.Instance | BindingFlags.Public);
            string typeName = derivedType.Name.Replace("_", " "); // Replace underscore with blank space
            typeName = typeName.Replace("Node", string.Empty);

            if (categoryProperty != null)
            {
                string category = (string)categoryProperty.GetValue(Activator.CreateInstance(derivedType));
                if (category.Equals("Hidden")) continue;

                if (!category.Equals(prev))
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(category), 1));
                    prev = category;
                }

                tree.Add(new SearchTreeEntry(new GUIContent(typeName))
                {
                    level = 2,
                    userData = derivedType
                });

            }
        }

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
    {
        var worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
        var localMousePosition = actionGraphView.contentViewContainer.WorldToLocal(worldMousePosition);

        // Check if this is an actual node entry (i.e. not a group entry)
        if (entry.userData is Type nodeType)
        {
            actionGraphView.CreateNode(nodeType, localMousePosition);
            Debug.Log($"Creating node at {localMousePosition}");
            return true;
        }

        return false;
    }

    public static List<Type> GetDerivedTypesOfActionNode()
    {
        List<Type> derivedTypes = new List<Type>();
        Type baseType = typeof(ActionNode<>);

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract && type.IsSubclassOfRawGeneric(baseType))
            {
                derivedTypes.Add(type);
            }
        }

        return derivedTypes;
    }

}

public static class TypeExtensions
{
    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }
}