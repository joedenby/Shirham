using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class Element_Node : ActionNode<ElementalType>
{
    public override string Category => "Value";
    public ElementalType elemental;

    public Element_Node() {
        operation = () => elemental;
    }

    public override Node GetNodeRepresentation()
    {
        Node node = new Node
        {
            title = "Element",
            style = { width = 250, height = 100 }
        };

        // Add dropdown
        EnumField dropdown = new EnumField("Element Type", elemental);
        dropdown.RegisterValueChangedCallback(evt =>
        {
            elemental = (ElementalType)evt.newValue;
        });

        node.mainContainer.Add(dropdown);

        //Add output port
        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(ElementalType));
        outputPort.portColor = new Color(0.8f, 0.1f, 0.8f);
        node.outputContainer.Add(outputPort);

        // Refresh the visual input and output ports to update their layout.
        node.RefreshExpandedState();
        node.RefreshPorts();

        return node;
    }

}
