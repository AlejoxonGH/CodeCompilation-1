using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePathfinding
{
    public List<Vector3> AStar(MazeNode start, MazeNode goal)
    {
        List<Vector3> path = new List<Vector3>();
        if (start == null || goal == null) return path;

        PriorityQueue<MazeNode> frontier = new PriorityQueue<MazeNode>();
        frontier.Enqueue(start, 0);

        Dictionary<MazeNode, MazeNode> cameFrom = new Dictionary<MazeNode, MazeNode>();
        cameFrom.Add(start, null);

        Dictionary<MazeNode, int> costSoFar = new Dictionary<MazeNode, int>();
        costSoFar.Add(start, 0);

        MazeNode current = default;

        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == goal) break;

            foreach (var next in current.Neighbors)
            {
                //if (next.Blocked) continue;
                int newCost = costSoFar[current] + next.Cost;
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

        //Armamos el camino
        while (current != null) //Si queremos el start pondriamos != null
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }

        return path;
    }

    float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b); //mas costosa pero mas precisa (puede entrar en conflicto con los pasos) 
        //return (b - a).sqrMagnitude; //menos costosa con el valor elevado al cuadrado
        //return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y); // Manhattan: usar para grillas
    }

}
