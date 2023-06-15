using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ActionNodeBase : Node
{
    public string GUID { get; set; }
    public Vector2 position { get; set; }

    public virtual string Category => "Default";

    public abstract Node GetNodeRepresentation();
}
