using UnityEngine;

public class AIPathNode : IHeapItem<AIPathNode>
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public AIPathNode parent;
    int heapIndex;

    public AIPathNode(Vector2 _worldPos, int _gridX, int _gridY)
    {
        walkable = !GameManager.Hub.Navigation.ObstacleAtLocation(_worldPos);
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(AIPathNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
