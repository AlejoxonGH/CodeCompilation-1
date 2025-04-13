using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Food : GridEntity
{
    [SerializeField] float _radius;
    [SerializeField] Queries _query;

    private void OnEnable()
    {
        UpdateGridPos();

        _query.radius = _radius;
        _query.targetGrid = SpatialGrid.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    protected override void Update()
    {
        if (_query.Query().Any())
            Destroy(gameObject);
    }

    private void OnDisable()
    {
        BoidManager.Instance.foodSpawned = false;
        SpatialGrid.Instance.Unsubscribe(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
