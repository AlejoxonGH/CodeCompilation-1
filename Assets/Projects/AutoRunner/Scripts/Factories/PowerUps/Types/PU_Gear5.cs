using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/SpeedUp")]
public class PU_Gear5 : PowerUps
{
    [SerializeField] float _speed;
    
    public override void Apply(RunnerPlayer p)
    {
        p.ChangeSpeed(_speed);

        Debug.Log("Speed Changed");
    }
}