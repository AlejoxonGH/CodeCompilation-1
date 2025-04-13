using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU : MonoBehaviour
{
    public PowerUps powerUps;

    public static void TurnOn(PU pU)
    {
        pU.gameObject.SetActive(true);

        Vector2 pos1 = PUFactory.Instance.spawnPoint1.position;
        Vector2 pos2 = PUFactory.Instance.spawnPoint2.position;
        pU.transform.position = new Vector2(pos1.x, Random.Range(pos1.y, pos2.y));
    }

    public static void TurnOff(PU pU)
    {
        pU.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7 || other.gameObject.tag == "Despawn")
        {
            Destroy(gameObject);
        }

        RunnerPlayer p = other.GetComponent<RunnerPlayer>();
        
        if (p != null)
        {
            Debug.Log("EL PLAYER ME TOCÓ :(");
            powerUps.Apply(p);
            Destroy(gameObject);
        }
    }
}