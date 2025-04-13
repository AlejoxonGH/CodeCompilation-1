using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static void TurnOn(Obstacle o)
    {
        o.gameObject.SetActive(true);

        if (o.GetComponent<Bird>())
            o.transform.position = ObstacleFactory.Instance.birdSpawnPoint.position;
        else
            o.transform.position = ObstacleFactory.Instance.obstacleSpawnPoint.position;
    }

    public static void TurnOff(Obstacle o)
    {
        o.gameObject.SetActive(false);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
            ObstacleFactory.Instance.ReturnObstacle(this);
    }
}
