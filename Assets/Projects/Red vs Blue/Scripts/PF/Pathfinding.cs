using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding
{
    private LayerMask _wallMask;
    readonly List<Vector3> EMPTY = new List<Vector3>();

    public Pathfinding(LayerMask wallMask) =>_wallMask = wallMask;

    List<Vector3> AStar(PvB_Node start, PvB_Node goal)
    {
        var path = new List<Vector3>();
        if (start == null || goal == null) return path;

        var frontier = new PriorityQueue<PvB_Node>();
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<PvB_Node, PvB_Node>();
        cameFrom.Add(start, null);

        var costSoFar = new Dictionary<PvB_Node, int>();
        costSoFar.Add(start, 0);

        PvB_Node current = default;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal) break;
            if (current.Neighbors.Count <= 0) Debug.LogError("NO NEIGHBORS ON THE LIST");

            foreach (var next in current.Neighbors)
            {
                var newCost = costSoFar[current] + next.Cost;

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, newCost + Heuristic(next.transform.position, goal.transform.position));
                    cameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);
                }
                else if (newCost < costSoFar[next])
                {
                    frontier.Enqueue(next, newCost + Heuristic(next.transform.position, goal.transform.position));
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                }
            }
        }

        if (current != goal) return path;

        while (current != null)
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }

        return path;
    }

    float Heuristic(Vector3 a, Vector3 b) => Vector3.Distance(a, b);

    public List<Vector3> ThetaStar(PvB_Node start, PvB_Node goal)
    {
        if (start == null || goal == null) return EMPTY;

        var path = AStar(start, goal);
        path.Reverse();

        var current = 0;

        while (current + 2 < path.Count)
        {
            if (path[current].InLineOfSight(path[current + 2], _wallMask)) //CAMBIAR A SPHERECAST SI SE PUEDE
                path.RemoveAt(current + 1);
            else
                current++;
        }

        return path;
    }
}