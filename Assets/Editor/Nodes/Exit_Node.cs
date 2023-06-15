using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Exit_Node : ActionNode<int>
{
    public override string Category => "Hidden";

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Exit Node",
            style = { width = 200, height = 120}
        };

        // Load the stylesheet and add the defined class to the node
        StyleSheet stylesheet = (StyleSheet)EditorGUIUtility.Load("Node.uss");
        node.styleSheets.Add(stylesheet);
        node.AddToClassList("exitNode");

        //Create int input port
        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(int));
        outputPort.portName = "Output";
        node.inputContainer.Add(outputPort);

        //Create BattleSquare[] input port
        Port selectionPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(BattleSquare[]));
        selectionPort.portName = "Selection";
        node.inputContainer.Add(selectionPort);

        //Create string input port
        Port animationPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(string));
        animationPort.portName = "Animation";
        node.inputContainer.Add(animationPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.SetPosition(new Rect(100, 200, 100, 150));

        // Refresh the visual input ports to update their layout.
        node.RefreshExpandedState();
        node.RefreshPorts();

        return node;
    }
}
