using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class MazeEnemy : FOVAgent
{
    #region VARIABLES
    /////////////////////////// VARIABLES //////////////////////////////

    MazeFSM _fsm;

    [SerializeField] Rigidbody _rb;
    [SerializeField] MazePlayer _player;
    [SerializeField] GameObject _enemiesParent;

    [SerializeField] float _speed;

    bool _alerted = false;
    bool _pfTravelingPath = false;

    Vector3 _alertPos;
    bool _gotToAlertPos = false;

    [SerializeField] List<MazeNode> _waypoints;
    int _wayIndex = 0;

    #endregion

    [SerializeField] GameObject _nodeParent;

    List<Vector3> _path = new List<Vector3>();
    MazePathfinding _pf = new MazePathfinding();

    MazeNode start;
    MazeNode goal;

    #region UNITY MONOBEHAVIOUR METHODS
    ////////////////////////// UNITY MONOBEHAVIOUR METHODS //////////////////////////

    void Start()
    {
        _fsm = new MazeFSM();

        _fsm.AddState(EnemyStates.Patrol, new PatrolState(this));
        _fsm.AddState(EnemyStates.Chase, new ChaseState(this));
        _fsm.AddState(EnemyStates.Alert, new AlertState(this));

        _fsm.ChangeState(EnemyStates.Patrol);
    }

    void Update()
    {
        _fsm.Update();
    }
    #endregion

    #region MOVEMENT METHODS
    ////////////////////////////// MOVEMENT METHODS /////////////////////////////////

    void FindPathToTarget(Vector3 target)
    {
        Vector3 myPos = transform.position;

        foreach (var node in _nodeParent.GetComponentsInChildren<MazeNode>())
        {
            Vector3 nodePos = node.gameObject.transform.position;

            if (start == null || goal == null)
            {
                if (start == null && goal == null)
                {
                    start = node;
                    goal = node;
                }
                else if (start == null)
                    start = node;
                else
                    goal = node;

                continue;
            }

            Vector3 startPos = start.gameObject.transform.position;
            Vector3 goalPos = goal.gameObject.transform.position;

            if (Vector3.Distance(nodePos, myPos) < Vector3.Distance(startPos, myPos))
                start = node;

            if (Vector3.Distance(nodePos, target) < Vector3.Distance(goalPos, target))
                goal = node;
        }
        
        _path = _pf.AStar(start, goal);
        _path?.Reverse();
        _pfTravelingPath = true;
    }

    void TravelPath()
    {
        if (_path?.Count > 0)
        {
            Vector3 desiredPos = _path[0]/* - Vector3.up*/;
            MoveToTarget(desiredPos);

            if (Vector3.Distance(desiredPos, transform.position) <= 0.05f)
                _path.RemoveAt(0);
        }
        else if (_path?.Count <= 0)
        {
            _pfTravelingPath = false;
        }
    }

    void PatrolWaypoints(Vector3 WPpos)
    {
        MoveToTarget(WPpos);

        if (Vector3.Distance(WPpos, transform.position) <= 0.05f)
        {
            _wayIndex++;
            if (_wayIndex >= _waypoints.Count) _wayIndex = 0;
        }
    }

    void ChasePlayer(Vector3 playerPos)
    {
        _alertPos = playerPos;
        MoveToTarget(playerPos);
    }

    void MoveToAlertedPosition()
    {
        MoveToTarget(_alertPos);

        if (Vector3.Distance(_alertPos, transform.position) <= 0.05f)
            _gotToAlertPos = true;
    }

    void MoveToTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;

        transform.up = dir;

        //_rb.MovePosition(target * _speed * Time.deltaTime);
        _rb.velocity = dir * _speed;
    }
    #endregion

    #region ALERT METHODS
    ////////////////////////////// ALERT METHODS /////////////////////////////////

    public void AlertAllEnemies()
    {
        foreach (var enemy in _enemiesParent.GetComponentsInChildren<MazeEnemy>())
        {
            enemy.AlertThisEnemy(_player.transform.position);
        }
    }

    public void AlertThisEnemy(Vector3 playerPos)
    {
        _alertPos = playerPos;
        _alerted = true;
    }

    public void UnalertThisEnemy()
    {
        _alerted = false;
    }
    #endregion

    #region FSM STATES ON ENTER METHODS
    ///////////////////////// FSM STATES ON ENTER METHODS /////////////////////////

    public void OnPatrolEnter()
    {
        if (!InLineOfSight(_waypoints[_wayIndex].transform.position))
        {
            FindPathToTarget(_waypoints[_wayIndex].transform.position);
        }
    }

    public void OnAlertEnter()
    {
        if (!InLineOfSight(_alertPos) && _alertPos != Vector3.zero)
        {
            FindPathToTarget(_alertPos);
        }

        _gotToAlertPos = false;
    }
    #endregion

    #region FSM STATES UPDATE METHODS
    ///////////////////////// FSM STATES UPDATE METHODS /////////////////////////

    public void PatrolLogic()
    {
        if (InFieldOfView(_player.transform.position))
        {   _fsm.ChangeState(EnemyStates.Chase);  return; }
        
        if (_alerted)
        {   _fsm.ChangeState(EnemyStates.Alert);  return; }

        if (_pfTravelingPath)
        {   TravelPath();  return; }


        if (InLineOfSight(_waypoints[_wayIndex].transform.position))
            PatrolWaypoints(_waypoints[_wayIndex].transform.position);
    }

    public void ChaseLogic()
    {
        Vector3 playerPos = _player.transform.position;

        if (!InFieldOfView(playerPos))
            _fsm.ChangeState(EnemyStates.Alert);
        else
            ChasePlayer(playerPos);
    }

    public void AlertLogic()
    {
        if (InFieldOfView(_player.transform.position))
        {   _fsm.ChangeState(EnemyStates.Chase);  return; }

        if (_pfTravelingPath)
        {   TravelPath();  return; }
        

        if (_gotToAlertPos)
            _fsm.ChangeState(EnemyStates.Patrol);
        else if (InLineOfSight(_alertPos) && _alertPos != Vector3.zero)
            MoveToAlertedPosition();
        else
        {   _gotToAlertPos = true;  _fsm.ChangeState(EnemyStates.Patrol); }
    }
    #endregion

    #region FSM STATES ON EXIT METHODS
    ///////////////////////// FSM STATES ON EXIT METHODS /////////////////////////

    public void OnAlertExit()
    {
        _alertPos = Vector3.zero;
    }
    #endregion
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 dirA = GetDirFromAngle(_viewAngle / 2 + transform.eulerAngles.z);
        Vector3 dirB = GetDirFromAngle(-_viewAngle / 2 + transform.eulerAngles.z);

        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * _viewRadius);

        Gizmos.color = Color.yellow;

        if (_path?.Count > 0)
        {
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }
        }
    }

    Vector3 GetDirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

public enum EnemyStates
{
    Patrol,
    Chase,
    Alert
}