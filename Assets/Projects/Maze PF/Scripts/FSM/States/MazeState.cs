using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MazeState
{
    public MazeFSM fsm; //Comodo pero problemas para finiteStateMachines genericas


    public abstract void OnEnter();
    public abstract void Update();
    public abstract void OnExit();
}
