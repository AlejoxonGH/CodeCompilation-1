using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUp : PickUp
{
    public override void GetPicked(BH_Player pj)
    {
        pj.GetCoins(_value);
    }
}
