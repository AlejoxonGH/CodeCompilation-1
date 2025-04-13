using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Shield")]
public class PU_Shield : PowerUps
{
    public override void Apply(RunnerPlayer p)
    {
        p.GrabShield();

        Debug.Log("Shielded");
    }
}