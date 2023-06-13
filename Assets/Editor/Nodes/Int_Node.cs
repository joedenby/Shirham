using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class Int_Node : ActionNode
{
    public override string Category => "Value";
    public int value;

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Int",
            style = { width = 100, height = 100 }
        };

        // Create an input field for the integer value
        IntegerField inputField = new IntegerField();
        inputField.RegisterValueChangedCallback(evt =>
        {
            // Handle the value change event
            value = evt.newValue;
            title = $"Int [{value}]";
        });

        node.mainContainer.Add(inputField);

        // Create an output port for the node
        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // Refresh the visual input and output ports to update their layout.
        RefreshExpandedState();
        RefreshPorts();

        return node;
    }
}
