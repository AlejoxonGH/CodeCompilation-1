using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeNode : MonoBehaviour
{
    [SerializeField] TextMeshPro _costText;

    [field : SerializeField] public List<MazeNode> Neighbors { get; private set; }
    public int Cost { get; private set; }

    void SetCost(int cost)
    {
        if (cost == 6) cost = 5;

        Cost = cost <= 0 ? 1 : cost;
        _costText.text = Cost.ToString();
        _costText.enabled = Cost > 1;
    }

    private void OnDrawGizmos()
    {
        foreach (var node in Neighbors)
        {
            Gizmos.DrawLine(transform.position, node.gameObject.transform.position);
        }
    }
}