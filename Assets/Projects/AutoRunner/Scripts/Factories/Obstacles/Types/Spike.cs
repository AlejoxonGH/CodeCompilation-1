using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Obstacle
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        RunnerPlayer p = collision.GetComponent<RunnerPlayer>();

        if (p != null)
            p.Die();
    }
}
