using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacle
{
    [SerializeField] Rigidbody2D _rgbd;
    
    private void OnEnable()
    {
        _rgbd.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnDisable()
    {
        _rgbd.bodyType = RigidbodyType2D.Static;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<RunnerBullet>())
        {
            ObstacleFactory.Instance.ReturnObstacle(this);
        }
        else
        {
            RunnerPlayer p = collision.gameObject.GetComponent<RunnerPlayer>();

            if (p != null)
                p.Die();
        }
    }
}
