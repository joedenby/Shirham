using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActionGraphView : GraphView
{
    public ActionBlueprint actionBlueprint;

    public ActionGraphView(ActionBlueprint blueprint)
    {
        // here we can put initialization logic for our custom GraphView
        actionBlueprint = blueprint;
    }

    // Create a new instance of a node (not to be confused with LoadNode)
    public void CreateNode(string nodeType, Vector2 position)
    {
        // create a new NodeData object
        NodeData newNodeData = new NodeData
        {
            nodeGUID = Guid.NewGuid().ToString(),
            nodeType = nodeType,
            position = position
        };

        // add the new NodeData to the ActionBlueprint
        actionBlueprint.nodeDataList.Add(null);

        // convert node data to a Node of the correct type
        Type type = Type.GetType(nodeType);
        ActionNode newNode = (ActionNode)Activator.CreateInstance(type);
        
        // finally, add node to the graph
        AddElement(newNode.GetNodeRepresentation());
    }
}