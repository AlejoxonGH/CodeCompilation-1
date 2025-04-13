using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PvB_Node : MonoBehaviour
{
    [SerializeField] TextMeshPro _costText;

    [SerializeField] int _neighborsMax;
    public List<PvB_Node> Neighbors { get; private set; } = new List<PvB_Node>();
    [field: SerializeField] public int Cost { get; private set; }
    
    [SerializeField] float _nearNodeRadius;
    [SerializeField] LayerMask _nodeMask;
    [SerializeField] LayerMask _obstacleAndWallLayer;

    private void Start()
    {
        var colliders = Physics.OverlapSphere(transform.position, _nearNodeRadius, _nodeMask, QueryTriggerInteraction.Collide);
        var posibleNeighbors = colliders.Where(x => x.GetComponent<PvB_Node>()).OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToArray();


        for (int i = 1; i < _neighborsMax + 1; i++)
        {
            if (transform.position.InLineOfSight(posibleNeighbors[i].transform.position, _obstacleAndWallLayer))
                Neighbors.Add(posibleNeighbors[i].GetComponent<PvB_Node>());
        }

        _costText.text = Cost.ToString();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _nearNodeRadius);

        foreach (var node in Neighbors)
            Gizmos.DrawLine(transform.position, node.gameObject.transform.position);
    }
}