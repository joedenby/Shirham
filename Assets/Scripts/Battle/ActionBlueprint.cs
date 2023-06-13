using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New ActionBlueprint", menuName = "Battle/ActionBlueprint")]
public class ActionBlueprint : ScriptableObject
{
    public List<NodeData> nodeDataList = new List<NodeData>();
    public List<EdgeData> edgeDataList = new List<EdgeData>();
}

[System.Serializable]
public class NodeData
{
    public string nodeGUID; // unique identifier for this node
    public string nodeType; // type of this node (Int, Elemental, Resistance, GetSquaresRadial, PlayUnitAnimation, etc.)
    public Vector2 position; // position of the node in the graph

    // For parameters, we can have a list of tuples. The first element in the tuple is the parameter type, and the second is the value.
    public List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

    // For ports, we need to store their name, type and whether they are essential or not
    public List<PortData> inputPorts = new List<PortData>();
    public List<PortData> outputPorts = new List<PortData>();
}

[System.Serializable]
public class PortData
{
    public string portName;
    public string portType; // the type of data this port handles
    public bool isEssential;
}

[System.Serializable]
public class EdgeData
{
    // Edge properties here
}