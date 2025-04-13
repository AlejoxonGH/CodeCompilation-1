using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUps : ScriptableObject
{
    public abstract void Apply(RunnerPlayer player);
}