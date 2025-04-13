using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MazeState
{
    MazeEnemy _enemy;

    public PatrolState(MazeEnemy e)
    {
        _enemy = e;
    }
    
    public override void OnEnter()
    {
        _enemy.OnPatrolEnter();
    }

    public override void Update()
    {
        _enemy.PatrolLogic();
    }

    public override void OnExit()
    {
    }

}
