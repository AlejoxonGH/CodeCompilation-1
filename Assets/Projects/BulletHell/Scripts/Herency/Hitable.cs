using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hitable : MonoBehaviour
{
    [Header("HEALTH POINTS")]

    [SerializeField] protected int _maxLife;
    public int MaxLife { get { return _maxLife; } }

    public int Life { get; protected set; }

    public virtual void TakeDamage(int dmg)
    {
        if ((Life - dmg) <= 0)
        {
            Life = 0;
            OnDeath();
        }
        else
            Life -= dmg;
    }

    public virtual void GetHealed(int heal)
    {
        if ((Life + heal) >= _maxLife)
            Life = _maxLife;
        else
            Life += heal;
    }

    public abstract void OnDeath();
}
