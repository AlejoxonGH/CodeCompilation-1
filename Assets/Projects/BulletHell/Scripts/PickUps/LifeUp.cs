using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUp : PickUp
{
    public override void GetPicked(BH_Player pj)
    {
        pj.GetHealed(_value);
    }
}
