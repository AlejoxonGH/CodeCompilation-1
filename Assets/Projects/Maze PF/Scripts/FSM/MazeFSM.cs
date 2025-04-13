using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFSM
{
    MazeState _currentState;

    //Coleccion de estados
    Dictionary<EnemyStates, MazeState> _allStates = new Dictionary<EnemyStates, MazeState>();

    public void ChangeState(EnemyStates key)
    {
        if(!_allStates.ContainsKey(key)) return;

        //if(_currentState)
        _currentState?.OnExit();
        _currentState = _allStates[key];
        _currentState.OnEnter();
    }


    public void AddState(EnemyStates key,MazeState state)
    {
        _allStates.Add(key, state);
        state.fsm = this;
    }

    public void Update()
    {
        if(_currentState == null) return;

        _currentState.Update();
    }

}
