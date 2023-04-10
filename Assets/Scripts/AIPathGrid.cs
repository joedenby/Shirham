using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AIPathGrid : MonoBehaviour
{
    public static AIPathGrid main;
    private void Awake() => main = this;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public AIPathNode[,] grid;
    public List<AIPathNode> path;
    public bool hasGrid => grid != null && grid.Length > 0;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    [Header("Debug")]
    public bool debugEnabled; 
    public bool showSelected;
    public Vector2 selected;
    public bool onlyDisplayPathGizmos;


    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public void Refresh()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                AIPathNode node = grid[x, y];
                node.gCost = int.MaxValue;
                node.parent = null;
            }
        }
    }

    void CreateGrid()
    {
        grid = new AIPathNode[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = new Vector2() { 
            x = transform.position.x - (gridSizeX / 2),
            y = transform.position.y - (gridSizeY / 2)
        };

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + (Vector2.right * (x * nodeDiameter + nodeRadius)) + (Vector2.up * (y * nodeDiameter + nodeRadius));
                grid[x, y] = new AIPathNode(worldPoint, x, y);
            }
        }
    }

    public List<AIPathNode> GetNeighbours(AIPathNode node)
    {
        List<AIPathNode> neighbours = new List<AIPathNode>();

        for (int x = -1; x <= 1; x++) {
            if (x == 0) continue;

            int checkX = node.gridX + x;
            if (checkX >= 0 && checkX < gridSizeX) {
                neighbours.Add(grid[checkX, node.gridY]);
            }
        }

        for (int y = -1; y <= 1; y++) {
            if (y == 0) continue;

            int checkY = node.gridY + y;
            if (checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(grid[node.gridX, checkY]);
            }
        }

        return neighbours;
    }

    public AIPathNode NodeFromWorldPoint(Vector2 worldPosition)
    {
        var pos = new Vector2(Mathf.FloorToInt(worldPosition.x) + 1, Mathf.FloorToInt(worldPosition.y) + 1);
        float percentX = (pos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (pos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Was rounded now changed to floor
        int x = Mathf.FloorToInt((gridSizeX - 1) * percentX);
        int y = Mathf.FloorToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public bool SameLocation(Vector2 a, Vector2 b) => NodeFromWorldPoint(a) == NodeFromWorldPoint(b);

    public AIPathNode RelativeNeighbourFromWorldPoint(Vector2 seeker, Vector2 target) {
        Vector2 dir = target + (seeker - target).normalized;
        return NodeFromWorldPoint(dir);
    }

    void OnDrawGizmos()
    {
        if(!debugEnabled) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0.1f));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (AIPathNode n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                List<AIPathNode> neighbours = GetNeighbours(NodeFromWorldPoint(selected));
                foreach (AIPathNode n in grid)
                {
                    if (showSelected && (n == NodeFromWorldPoint(selected) || neighbours.Contains(n)) && n.walkable)
                    {
                        Gizmos.color = (neighbours.Contains(n)) ? new Color(0, 1, 0, 0.5f) : Color.green;
                    }
                    else
                    {
                        Gizmos.color = (n.walkable) ? new Color(1, 1, 1, 0.5f) : Color.red;
                    }

                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }

            }
        }
    }

}
