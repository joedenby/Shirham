using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class ActionNode : Node
{
    public string GUID;
    public Vector2 position;
    public abstract string Category { get; }

    public abstract Node GetNodeRepresentation();

}
