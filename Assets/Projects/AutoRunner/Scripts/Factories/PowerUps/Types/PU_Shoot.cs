using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Shoot")]
public class PU_Shoot : PowerUps
{
    public override void Apply(RunnerPlayer p)
    {
        p.GrabGun();

        Debug.Log("Armed");
    }
}