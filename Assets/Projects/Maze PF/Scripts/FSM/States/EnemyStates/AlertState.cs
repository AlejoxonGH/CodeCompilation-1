using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : MazeState
{
    MazeEnemy _enemy;

    public AlertState(MazeEnemy e)
    {
        _enemy = e;
    }

    public override void OnEnter()
    {
        _enemy.OnAlertEnter();
    }

    public override void Update()
    {
        _enemy.AlertLogic();
    }

    public override void OnExit()
    {
        _enemy.UnalertThisEnemy();
        _enemy.OnAlertExit();
    }
}
