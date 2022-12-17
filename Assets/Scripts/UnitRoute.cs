using GameManager.Hub;
using UnityEngine;


[System.Serializable]
public class UnitRoute
{
    public Route route;
    public enum Route { Static, Random, Patrol, Cycle }
    public Vector2[] routePoints;
    public float delay = 1f;
    public bool waiting { private set; get; }
    public bool hasWaited { private set; get; }
    public Vector2 sequence { private set; get; }


    public UnitRoute() {
        route = Route.Static;
        routePoints = new Vector2[0];
    }

    public UnitRoute(Route route, Vector2[] routePoints)
    {
        this.route = route;
        this.routePoints = routePoints;
    }

    public Vector2 GetNext()
    {
        if (waiting || routePoints.Length == 0) 
            return Vector2.zero;

        if (!hasWaited) {
            CallWait();
            return Vector2.zero;
        }

        hasWaited = false;
        //At end and not static, update needed
        switch (route)
        {
            case Route.Random:
                if (routePoints == null || routePoints.Length == 0)
                    return Vector2.zero;

                int x = Random.Range(0, routePoints.Length);
                var next = routePoints[x];
                return Navigation.ObstacleAtLocation(next) ? Vector2.zero : next;
            case Route.Patrol:
                if (sequence.x + 1 >= routePoints.Length && sequence.y == 0) {
                    sequence = new Vector2(sequence.x - 1, 1);
                } else if (sequence.x - 1 < 0 && sequence.y == 1) {
                    sequence = new Vector2(0, 0);
                }
                else {
                    sequence = new Vector2(sequence.y > 0 ? sequence.x - 1 : sequence.x + 1, sequence.y);
                }

                next = routePoints[(int)sequence.x];
                return Navigation.ObstacleAtLocation(next) ? Vector2.zero : next;
            case Route.Cycle:
                sequence = new Vector2((sequence.x + 1 >= routePoints.Length) ? 0 : sequence.x + 1, 0);
                next = routePoints[(int)sequence.x];
                return Navigation.ObstacleAtLocation(next) ? Vector2.zero : next;
        }

        return Vector2.zero;
    }

    public async void CallWait() { 
        if(waiting) return;
        waiting = true;
        await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(delay * 1000));
        waiting = false;
        hasWaited = true;
    }

    public bool IsStatic() => route.Equals(Route.Static);
    
}
