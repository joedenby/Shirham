using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Int_Node : ActionNode<int>
{
    public override string Category => "Value";
    public int value;

    public Int_Node() {
        operation = () => value;
    }

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Int",
            style = { width = 90, height = 100 }
        };

        node.inputContainer.style.display = DisplayStyle.None;
        node.mainContainer.style.flexDirection = FlexDirection.Column; // Stack children vertically

        // Create a spacer to push the input field down
        VisualElement spacer = new VisualElement();
        spacer.style.flexGrow = 1;
        node.mainContainer.Add(spacer);

        // Create an input field for the integer value
        IntegerField inputField = new IntegerField();
        inputField.style.unityTextAlign = TextAnchor.MiddleCenter; // Center the text
        inputField.style.width = 50; // Reduce width
        inputField.style.height = 20; // Increase height
        inputField.style.alignSelf = Align.Center; // Center the field horizontally

        inputField.RegisterValueChangedCallback(evt =>
        {
            // Handle the value change event
            value = evt.newValue;
        });

        node.mainContainer.Add(inputField);

        node.mainContainer.Add(inputField);

        // Create an output port for the node
        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(int));
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // Refresh the visual input and output ports to update their layout.
        RefreshExpandedState();
        RefreshPorts();

        return node;
    }
}
