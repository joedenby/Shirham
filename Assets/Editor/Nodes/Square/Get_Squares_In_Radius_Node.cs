using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class Get_Squares_In_Radius_Node : ActionNode
{
    public override string Category => "Square";

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Get Squares In Radius",
            style = { width = 200, height = 100 }
        };

        //Create Vector2 center point port
        Port v2Port = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(UnityEngine.Vector2));
        v2Port.name = "Center";
        node.inputContainer.Add(v2Port);

        //Create int input port
        Port intInput = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(int));
        intInput.name = "Radius";
        node.inputContainer.Add(intInput);

        // Create an output port for the node
        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(BattleSquare[]));
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // Refresh the visual input and output ports to update their layout.
        node.RefreshExpandedState();
        node.RefreshPorts();

        return node;
    }
}