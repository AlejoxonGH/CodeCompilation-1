using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var lead = other.GetComponent<Leader>();

        if (lead != null)
        {
            if (lead.MyBoidTeam == RvB_Boid.BoidTeam.RED)
                SceneManager.LoadScene(6);
            else
                SceneManager.LoadScene(7);
        }
    }
}
