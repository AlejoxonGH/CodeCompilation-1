using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : MazeState
{
    MazeEnemy _enemy;

    public ChaseState(MazeEnemy e)
    {
        _enemy = e;
    }

    public override void OnEnter()
    {
        _enemy.AlertAllEnemies();
    }

    public override void Update()
    {
        _enemy.ChaseLogic();
    }

    public override void OnExit()
    {
    }

}
