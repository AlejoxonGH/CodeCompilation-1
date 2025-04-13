using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVAgent : MonoBehaviour
{
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] protected float _viewRadius;
    [SerializeField][Range(0, 360)] protected float _viewAngle;


    protected bool InFieldOfView(Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;
        if (dir.sqrMagnitude > _viewRadius * _viewRadius) return false; //distance
        if (Vector3.Angle(transform.up, dir) > _viewAngle / 2) return false;
        if (!InLineOfSight(endPos)) return false;
        return true;
    }

    protected bool InLineOfSight(Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;
        return !Physics.Raycast(transform.position, dir, dir.magnitude, _wallLayer);
    }
}