using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public class Leader : RvB_Boid
{
    [Space(40)]
    [Header("LEADER")] //////////////////////////////////////////////////////// LEADER //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] GameObject _goalNodePrefab;
    GameObject _currentGoal = null;
    Vector3 _mouseWorldPos;

    public override void GetHealed(int heal)
    {
        if (!_canGetHealed) return;

        Life += heal;

        if (Life >= _highHealth)
        {
            if (Life >= MaxLife)
                Life = MaxLife;

            if (_myFsm.Current.Name == "FLEE")
            {
                if (ThereIsACloseEnemyInFOV())
                    SendInputToFSM(BoidStates.ATTACK);
                else if (_currentGoal == null)
                    SendInputToFSM(BoidStates.MOVE);
                else
                    SendInputToFSM(BoidStates.IDLE);
            }
        }

        ShowLifeText($"+{heal}", Color.green);

        StartCoroutine(HealCooldown());
    }

    protected override void Start()
    {
        base.Start();

        //SETEO INICIAL

        var idle = new RvB_State<BoidStates>("IDLE");
        var move = new RvB_State<BoidStates>("MOVE");
        var attack = new RvB_State<BoidStates>("ATTACK");
        var flee = new RvB_State<BoidStates>("FLEE");

        StateConfigurer.Create(idle)
            .SetTransition(BoidStates.MOVE, move)
            .SetTransition(BoidStates.ATTACK, attack)
            .SetTransition(BoidStates.FLEE, flee)
            .Done();

        StateConfigurer.Create(move)
            .SetTransition(BoidStates.IDLE, idle)
            .SetTransition(BoidStates.ATTACK, attack)
            .SetTransition(BoidStates.FLEE, flee)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(BoidStates.IDLE, idle)
            .SetTransition(BoidStates.MOVE, move)
            .SetTransition(BoidStates.FLEE, flee)
            .Done();

        StateConfigurer.Create(flee)
            .SetTransition(BoidStates.IDLE, idle)
            .SetTransition(BoidStates.MOVE, move)
            .SetTransition(BoidStates.ATTACK, attack)
            .Done();
        


        //SETEO ESTADOS

        //IDLE
        idle.OnEnter += x =>
        {
            _rb.velocity = Vector3.zero;
            _rb.rotation = Quaternion.identity;
            _rend.material.color = Color.white;
        };
        idle.OnUpdate += () =>
        {
            if (ThereIsACloseEnemyInFOV() || _pursuitTarget != null)
            {
                if (_myFsm.Current.Name == "FLEE") return;
                SendInputToFSM(BoidStates.ATTACK); return;
            }

            if (_currentGoal != null)
            {
                if (_myFsm.Current.Name == "FLEE") return;
                SendInputToFSM(BoidStates.MOVE);
            }
        };
        

        //MOVE
        move.OnEnter += x =>
        {
            _rend.material.color = Color.green;
        };
        move.OnUpdate += () => 
        {
            if (ThereIsACloseEnemyInFOV() || _pursuitTarget != null)
            {
                if (_myFsm.Current.Name == "FLEE") return;
                SendInputToFSM(BoidStates.ATTACK); return;
            }

            if (_currentGoal == null)
            {
                if (_myFsm.Current.Name == "FLEE") return;
                SendInputToFSM(BoidStates.IDLE); return;
            }

            if (Vector3.Distance(transform.position, _currentGoal.transform.position) <= _touchArriveGoalRadius)
            {
                Destroy(_currentGoal); _currentGoal = null;

                if (_myFsm.Current.Name == "FLEE") return;
                SendInputToFSM(BoidStates.IDLE); return;
            }

            MoveToGoal();
        };
        move.OnFixedUpdate += () => 
        {
            FixedMove();
        };
        move.OnExit += x => 
        {
            
        };
       

        //ATTACK
        attack.OnEnter += x => 
        {
            _rend.material.color = Color.magenta;
        };
        attack.OnUpdate += () =>
        {
            ThereIsACloseEnemyInFOV();

            if (_pursuitTarget != null)
            {
                if (_canShoot) Shoot();
                AddForce(Pursuit(_pursuitTarget, _maxSpeed / 5));
                return;
            }

            if (_currentGoal == null)
            {
                if (_myFsm.Current.Name == "FLEE") return;
                SendInputToFSM(BoidStates.IDLE); return;
            }

            if (_myFsm.Current.Name == "FLEE") return;
            SendInputToFSM(BoidStates.MOVE);
        };
        attack.OnFixedUpdate += () =>
        {
            FixedMove(_pursuitTarget.transform.position - transform.position);
        };
        attack.OnExit += x =>
        {

        };

        //FLEE
        flee.OnEnter += x =>
        {
            _rend.material.color = Color.yellow;
        };
        flee.OnUpdate += () =>
        {
            ThereIsACloseEnemyInFOV();

            if (_pursuitTarget != null)
            { AddForce(Evade(_pursuitTarget)); return; }

            MoveToGoal(_healingPoint.gameObject);
        };
        flee.OnFixedUpdate += () =>
        {
            if (Vector3.Distance(transform.position, _healingPoint.transform.position) > _touchArriveGoalRadius)
            { FixedMove(); return; }

            if (_rb.velocity != Vector3.zero)
                _rb.velocity = Vector3.zero;
        };
        flee.OnExit += x =>
        {

        };

        //CREACION FSM
        _myFsm = new EventFSM<BoidStates>(idle);
    }
    protected override void Update()
    {
        if (MyMouseInputIsClicked()) CreateNodeToMove();

        base.Update();
    }
    void FixedUpdate() => _myFsm.FixedUpdate();


    #region FOR MOVEMENT
    bool MyMouseInputIsClicked()
    {
        if (MyBoidTeam == BoidTeam.RED)
        {
            if (Input.GetMouseButtonDown(0))
                return true;

            return false;
        }
        
        if (Input.GetMouseButtonDown(1))
            return true;

        return false;
    }

    void CreateNodeToMove()
    {
        if (_currentGoal != null)
        { Destroy(_currentGoal.gameObject); _currentGoal = null; }

        var mouseCamPos = Input.mousePosition;
        mouseCamPos.z = -Camera.main.transform.position.z;

        _mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseCamPos);
        _mouseWorldPos.z = 0;

        _currentGoal = Instantiate(_goalNodePrefab, _mouseWorldPos, Quaternion.identity);
    }

    void MoveToGoal() => MoveToGoal(_currentGoal);
    void MoveToGoal(GameObject goal)
    {
        if (goal == null) return;

        if (transform.position.InLineOfSight(goal.transform.position, _obstacleAndWallLayer))
        {
            if (_pfTravelingPath) _pfTravelingPath = false;
            AddForce(Arrive(goal.transform.position));
            return;
        }

        if (!_pfTravelingPath)
            FindPathToTarget(goal.transform.position);

        TravelPath();
    }
    #endregion

    void OnCollisionStay(Collision collision)
    {
        var boid = collision.gameObject.GetComponent<RvB_Boid>();
        if (boid != null)
        {
            if (boid.MyBoidTeam == MyBoidTeam) return;
            if (boid is Leader) boid.TakeDamage(10);
        }
    }
}