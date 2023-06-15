using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class Add_Node : ActionNode<int>
{
    public override string Category => "Operator";

    public ActionNode<int> intANode { get; set; }
    public ActionNode<int> intBNode { get; set; }


    public Add_Node() {
        operation = () => (intANode.Evaluate() + intBNode.Evaluate());
    }

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Add",
            style = { width = 150, height = 95 }
        };

        var inputAPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(int));
        var inputBPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(int));
        node.inputContainer.Add(inputAPort);
        node.inputContainer.Add(inputBPort);
         
        // Add output port for Vector2
        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(int));
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // Refresh the visual input and output ports to update their layout.
        node.RefreshExpandedState();
        node.RefreshPorts();

        return node;
    }

}
