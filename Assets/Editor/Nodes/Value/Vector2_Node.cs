using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class Vector2_Node : ActionNode<Vector2>
{
    private FloatField xField;
    private FloatField yField;
    private Vector2 vector2Value;
    public override string Category => "Value";

    public Vector2_Node() {
        operation = () => vector2Value;
    }

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Vector2",
            style = { width = 150, height = 95 }
        };

        // Initialize FloatFields for X and Y
        xField = new FloatField();
        yField = new FloatField();

        // Register value change callbacks for x and y
        xField.RegisterValueChangedCallback(evt =>
        {
            vector2Value.x = evt.newValue;
        });
        xField.style.width = 40;

        yField.RegisterValueChangedCallback(evt =>
        {
            vector2Value.y = evt.newValue;
        });
        yField.style.width = 40;

        // Create a custom VisualElement for each field with a Label and FloatField
        var xContainer = new VisualElement();
        xContainer.style.flexDirection = FlexDirection.Row;
        xContainer.Add(new Label("X") { style = { width = 10 } });
        xContainer.Add(xField);

        var yContainer = new VisualElement();
        yContainer.style.flexDirection = FlexDirection.Row;
        yContainer.Add(new Label("Y") { style = { width = 10 } });
        yContainer.Add(yField);

        // Create a new VisualElement to use as a background for the input fields
        var inputBackground = new VisualElement();
        inputBackground.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f); // This could be any color
        inputBackground.style.flexDirection = FlexDirection.Row;  // Aligns children horizontally
        inputBackground.style.justifyContent = Justify.Center; // Centers the children
        inputBackground.Add(xContainer);
        inputBackground.Add(yContainer);

        // Add float fields to input container
        node.mainContainer.Add(inputBackground);

        // Add output port for Vector2
        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Vector2));
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // Refresh the visual input and output ports to update their layout.
        node.RefreshExpandedState();
        node.RefreshPorts();

        return node;
    }
}
