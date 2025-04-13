using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Tools
{
    public static bool InLineOfSight(this Vector3 start, Vector3 end, LayerMask mask)
    {
        var dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, mask);
    }

    public static bool InFieldOfView(this Vector3 startPos, Vector3 endPos, LayerMask mask, float viewRadius, float viewAngle, Vector3 transRights)
    {
        var dir = endPos - startPos;

        if (dir.sqrMagnitude > viewRadius * viewRadius) return false;
        if (Vector3.Angle(transRights, dir) > viewAngle / 2) return false;
        if (Physics.Raycast(startPos, dir, dir.magnitude, mask)) return false;

        return true;
    }

    public static RvB_SteeringAgent FindClosestTo(this IEnumerable<RvB_SteeringAgent> coll, Vector3 mainPosition)
    {
        if (!coll.Any()) { Debug.LogError("Collection Empty"); return default; }

        RvB_SteeringAgent closest = default;

        foreach (var item in coll)
        {
            if (item == null || item == default)
            { Debug.Log("item IS default OR null"); closest = null; continue; }
            
            if (closest == default)
            { closest = item; continue; }
            
            if (Vector3.Distance(item.transform.position, mainPosition) < Vector3.Distance(closest.transform.position, mainPosition))
                closest = item;
        }

        if (closest == null || closest == default) { Debug.LogError("\"BruhWhat\"-Type of Error"); return default; }

        return closest;
    }
}