using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ActionBlueprint", menuName = "Battle/ActionBlueprint")]
public class ActionBlueprint : ScriptableObject
{
    public List<NodeData> nodeDataList = new List<NodeData>();
    public List<EdgeData> edgeDataList = new List<EdgeData>();
}

[System.Serializable]
public class NodeData
{
    // Node properties here
}

[System.Serializable]
public class EdgeData
{
    // Edge properties here
}