using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionGraphView : GraphView
{
    public ActionBlueprint actionBlueprint;
    public EditorWindow editorWindow;

    public ActionGraphView(EditorWindow editorWindow, ActionBlueprint blueprint)
    {
        this.editorWindow = editorWindow;
        actionBlueprint = blueprint;


        // This makes the graph view zoomable.
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // This allows you to select nodes and move them around.
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // This allows you to pan the view using the middle mouse button.
        this.AddManipulator(new ContentDragger());

        AddSearchWindow(editorWindow);
        AddStyle();
    }

    private void AddStyle() {

        GridBackground grid = new GridBackground();
        grid.StretchToParentSize();
        Insert(0, grid);

        StyleSheet stylesheet = (StyleSheet)EditorGUIUtility.Load("ActionGraphView.uss");
        styleSheets.Add(stylesheet);
    }

    // Create a new instance of a node (not to be confused with LoadNode)
    public void CreateNode(Type nodeType, Vector2 position)
    {
        // Create a new instance of the specified node type
        ActionNode baseNodeInstance = (ActionNode)Activator.CreateInstance(nodeType);

        // create a new NodeData object
        NodeData newNodeData = new NodeData
        {
            nodeGUID = Guid.NewGuid().ToString(),
            nodeType = nodeType.Name,
            position = position
        };

        // Initialize the nodeDataList if it is null
        if (actionBlueprint.nodeDataList == null)
        {
            actionBlueprint.nodeDataList = new List<NodeData>();
        }

        // add the new NodeData to the ActionBlueprint
        actionBlueprint.nodeDataList.Add(newNodeData);

        // Set the GUID of the new node
        baseNodeInstance.GUID = newNodeData.nodeGUID;

        //Set pickingMode to Pickingmode.Position
        baseNodeInstance.pickingMode = PickingMode.Position;

        // Add the node to the graph
        AddElement(baseNodeInstance.GetNodeRepresentation());
    }

    public void LoadBlueprint(ActionBlueprint blueprint)
    {
        // Clear the current view
        this.Clear();

        if (blueprint.nodeDataList is not null) {
            // For each NodeData in the blueprint
            foreach (NodeData nodeData in blueprint.nodeDataList)
            {
                // Create the corresponding ActionNode
                Type nodeType = Type.GetType(nodeData.nodeType);
                ActionNode newNode = (ActionNode)Activator.CreateInstance(nodeType);

                // Add the new node to the GraphView
                AddElement(newNode.GetNodeRepresentation());
            }
        }

        // Additional logic to create connections between nodes based on the blueprint
        Debug.Log($"Loaded blueprint: {blueprint.name}");
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node) { 
                if(port.portType == startPort.portType) 
                    compatiblePorts.Add(port); 
            }
        });

        return compatiblePorts;
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        var _searchWindow = ScriptableObject.CreateInstance<ActionSearchTree>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = contect => SearchWindow.Open(new SearchWindowContext(contect.screenMousePosition), _searchWindow);
    }
}