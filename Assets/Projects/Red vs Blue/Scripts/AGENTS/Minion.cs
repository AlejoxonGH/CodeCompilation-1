using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public class Minion : RvB_Boid
{
    [Space(40)]
    [Header("MINION")] //////////////////////////////////////////////////////// MINION //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] Leader _myLeader;

    [SerializeField] float _separationRadius;
    [SerializeField][Range(0f, 5f)] float _separationWeight;
    [SerializeField][Range(0f, 3f)] float _alignmentWeight;
    [SerializeField][Range(0f, 3f)] float _cohesionWeight;

    protected override void Start()
    {
        base.Start();

        //SETEO INICIAL

        var move = new RvB_State<BoidStates>("MOVE");
        var attack = new RvB_State<BoidStates>("ATTACK");
        var flee = new RvB_State<BoidStates>("FLEE");


        StateConfigurer.Create(move)
            .SetTransition(BoidStates.ATTACK, attack)
            .SetTransition(BoidStates.FLEE, flee)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(BoidStates.MOVE, move)
            .SetTransition(BoidStates.FLEE, flee)
            .Done();

        StateConfigurer.Create(flee)
            .SetTransition(BoidStates.MOVE, move)
            .SetTransition(BoidStates.ATTACK, attack)
            .Done();


        //SETEO ESTADOS


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

            if (_myLeader != null)
            { LeaderFollowing(); return; }

            Flocking();
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
                LeaderFollowing();
                return;
            }

            if (_myFsm.Current.Name == "FLEE") return;
            SendInputToFSM(BoidStates.MOVE);
        };
        attack.OnFixedUpdate += () =>
        {
            if (_myLeader != null)
            {
                FixedMove(_pursuitTarget.transform.position - transform.position);
                return;
            }

            FixedMove();
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

            MoveToGoal(_healingPoint.gameObject, false, null);
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
        _myFsm = new EventFSM<BoidStates>(move);
    }
    void FixedUpdate() => _myFsm.FixedUpdate();

    #region MOVEMENT
    void MoveToGoal(GameObject goal, bool useSeparation, HashSet<RvB_Boid> boidsColl)
    {
        if (goal == null) return;

        if (transform.position.InLineOfSight(goal.transform.position, _obstacleAndWallLayer))
        {
            if (_pfTravelingPath) _pfTravelingPath = false;
            AddForce(Arrive(goal.transform.position));
            if (useSeparation) AddForce(Separation(boidsColl) * _separationWeight);

            return;
        }

        if (!_pfTravelingPath) FindPathToTarget(goal.transform.position);
        TravelPath();
    }

    void LeaderFollowing()
    {
        if (_myLeader == null) return;

        if (MyBoidTeam == BoidTeam.RED)
        { MoveToGoal(_myLeader?.gameObject, true, RvB_GameManager.Instance.RedTeamBoids); return; }

        MoveToGoal(_myLeader?.gameObject, true, RvB_GameManager.Instance.BlueTeamBoids);
    }
    #endregion

    #region FLOCKING
    void Flocking()
    {
        if (MyBoidTeam == BoidTeam.RED)
        {
            AddForce(Separation(RvB_GameManager.Instance.RedTeamBoids) * _separationWeight);
            AddForce(Alignment(RvB_GameManager.Instance.RedTeamBoids) * _alignmentWeight);
            AddForce(Cohesion(RvB_GameManager.Instance.RedTeamBoids) * _cohesionWeight);
            return;
        }

        AddForce(Separation(RvB_GameManager.Instance.BlueTeamBoids) * _separationWeight);
        AddForce(Alignment(RvB_GameManager.Instance.BlueTeamBoids) * _alignmentWeight);
        AddForce(Cohesion(RvB_GameManager.Instance.BlueTeamBoids) * _cohesionWeight);
    }

    Vector3 Separation(HashSet<RvB_Boid> boids) => Separation(boids, _separationRadius);
    Vector3 Separation(HashSet<RvB_Boid> boids, float radius)
    {
        var desired = Vector3.zero;

        foreach (var item in boids)
        {
            var dist = item.transform.position - transform.position;
            if (item == this || dist.magnitude > radius) continue;

            desired += dist;
        }

        if (desired == Vector3.zero) return desired;
        return CalculateSteering((desired * -1).normalized * _maxSpeed);
    }

    Vector3 Alignment(HashSet<RvB_Boid> boids)
    {
        var desired = Vector3.zero;
        var count = 0;

        foreach (var item in boids)
        {
            if (item == this) continue;
            if (Vector3.Distance(transform.position, item.transform.position) <= _viewRadius)
            { desired += item.Velocity; count++; }
        }

        if (count == 0) return Vector3.zero;
        return CalculateSteering((desired / count).normalized * _maxSpeed);
    }

    Vector3 Cohesion(HashSet<RvB_Boid> boids)
    {
        var desiredPos = Vector3.zero;
        var count = 0;

        foreach (var item in boids)
        {
            if (Vector3.Distance(transform.position, item.transform.position) > _viewRadius) continue;
            if (item != this) count++;

            desiredPos += item.transform.position;
        }

        if (count == 0) return Vector3.zero;
        return Seek(desiredPos / boids.Count);
    }
    #endregion

    void OnCollisionEnter(Collision collision)
    {
        var boid = collision.gameObject.GetComponent<RvB_Boid>();
        if (boid != null)
        {
            if (boid.MyBoidTeam == MyBoidTeam) return;

            if (boid is Minion)
                boid.TakeDamage(9999);
            else
                boid.TakeDamage(Life);

            TakeDamage(9999);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = new Color(1, 0.02f, 0.6f, 1); // PURPLE
        Gizmos.DrawWireSphere(transform.position, _separationRadius);
    }
}
