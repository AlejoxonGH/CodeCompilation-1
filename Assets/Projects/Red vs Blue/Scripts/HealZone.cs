using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    [field: SerializeField] public RvB_Boid.BoidTeam MyBoidTeam { get; private set; }
    [SerializeField] int _healAmount;

    void OnTriggerStay(Collider other)
    {
        var boid = other.GetComponent<RvB_Boid>();
        if (boid != null)
        {
            if (boid.MyBoidTeam != MyBoidTeam) return;

            if (boid is Leader)
                boid.GetComponent<Leader>().GetHealed(_healAmount);
            else
                boid.GetHealed(_healAmount);
        }
    }
}
