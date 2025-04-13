using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    [SerializeField] protected int _value;

    public abstract void GetPicked(BH_Player pj);

    private void OnTriggerEnter(Collider other)
    {
        var p = other.GetComponent<BH_Player>();

        if (p != null)
        {
            GetPicked(p);
            Destroy(gameObject);
        }
    }
}
