using System.Collections.Generic;
using UnityEngine;

public class AIPathFinder : MonoBehaviour
{
    private Vector2 target { get; set; }
    public bool targetLock;
    public bool hasPath => (path == null || path.Count > 0);
    public int pathLength => (path == null) ? 0 : path.Count;
    public bool nextToTarget = false;

    public Heap<AIPathNode> Open;
    public List<AIPathNode> Closed = new List<AIPathNode>();
    public Queue<Vector2> path;

    private Transform targetTransform;


    private async void Start()
    {
        GoTo(target);
    }

    public void GoTo(Transform target, bool follow = false) {
        targetTransform = target;
        targetLock = false;
        if (target == null) return;

        FindPath();
    }
    public void GoTo(Vector2 target) {
        this.target = target;
        FindPath();
    }

    public Vector2 NextPoint() {
        if (path == null || path.Count == 0) {
            return transform.position;
        }

        return path.Dequeue();
    }

    public void Clear() {
        path.Clear();
        StopAllCoroutines();
        target = Vector2.zero;
        targetTransform = null;
    }

    private void FindPath() {
        if (!AIPathGrid.main || !AIPathGrid.main.hasGrid) return;
        AIPathNode start = AIPathGrid.main.NodeFromWorldPoint(transform.position);
        AIPathNode end = nextToTarget ? AIPathGrid.main.RelativeNeighbourFromWorldPoint(transform.position, GetTarget()) :
            AIPathGrid.main.NodeFromWorldPoint(GetTarget());

        if (start == end) return;

        AIPathGrid.main.Refresh();
        Closed.Clear();
        path = new Queue<Vector2>();
        Open = new Heap<AIPathNode>(AIPathGrid.main.MaxSize);
        Open.Add(start);

        start.gCost = 0;
        start.hCost = GetDistance(start, end);

        while (Open.Count > 0)
        {
            AIPathNode current = Open.RemoveFirst();
            if (current == end)
            {
                BuildPath(current);
                return;
            }

            Closed.Add(current);
            List<AIPathNode> neighbours = AIPathGrid.main.GetNeighbours(current);
            foreach (AIPathNode neighbour in neighbours)
            {
                if (!neighbour.walkable || Closed.Contains(neighbour)) continue;

                int gCost = neighbour.gCost + GetDistance(current, neighbour);
                if (gCost < neighbour.gCost)
                {
                    neighbour.parent = current;
                    neighbour.gCost = gCost;
                    neighbour.hCost = GetDistance(neighbour, end);

                    if (!Open.Contains(neighbour))
                    {
                        Open.Add(neighbour);
                    }

                }
            }
        }
    }

    public Vector2 GetTarget() => targetTransform ? targetTransform.position : target;

    private void BuildPath(AIPathNode end) {
        AIPathGrid.main.path = new List<AIPathNode> { end };
        AIPathNode current = end;

        while (current.parent != null) {
            AIPathGrid.main.path.Add(current.parent);
            current = current.parent;
        }

        AIPathGrid.main.path.Reverse();
        path = new Queue<Vector2>();
        AIPathGrid.main.path.ForEach(x => path.Enqueue(x.worldPosition));
    }

    private int GetDistance(AIPathNode nodeA, AIPathNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

}
