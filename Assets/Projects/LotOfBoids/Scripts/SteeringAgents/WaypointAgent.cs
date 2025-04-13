using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA2;

public class WaypointAgent : LOB_SteeringAgent
{
    //[SerializeField] SpatialGrid _grid;
    
    public Transform[] waypoints;

    private int _currentWaypoint;

    public float waypointDistance;

    public float maxStamina;
    float _stamina;
    public float Stamina { get { return _stamina; } }
    
    [SerializeField] Queries _queries;

    public enum HunterInputs { PATROL, CHASE, REST }
    EventFSM<HunterInputs> _fsm;
    MeshRenderer _render;

    #region MONOBEHAVIOUR METHODS (Start, Update, etc.)
    void Awake()
    {
        _render = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        _stamina = maxStamina;


        _queries.radius = pursuitRadius;



        var PATROL = new State<HunterInputs>("PATROL");
        var CHASE = new State<HunterInputs>("CHASE");
        var REST = new State<HunterInputs>("REST");




        StateConfigurer.Create(PATROL)
            .SetTransition(HunterInputs.CHASE, CHASE)
            .SetTransition(HunterInputs.REST, REST)
            .Done();

        StateConfigurer.Create(CHASE)
            .SetTransition(HunterInputs.PATROL, PATROL)
            .SetTransition(HunterInputs.REST, REST)
            .Done();

        StateConfigurer.Create(REST)
            .SetTransition(HunterInputs.CHASE, CHASE)
            .SetTransition(HunterInputs.PATROL, PATROL)
            .Done();



        PATROL.OnEnter += x =>
        {
            _render.material.color = Color.yellow;
        };

        PATROL.OnUpdate += () =>
        {
            if (Stamina > 0)
            {
                var closestBoid = GetClosestBoid();

                if (closestBoid == null)
                    Patrol();
                else
                    SendInputToFSM(HunterInputs.CHASE);
            }
            else
                SendInputToFSM(HunterInputs.REST);
        };




        CHASE.OnEnter += x =>
        {
            _render.material.color = Color.red;
        };

        CHASE.OnUpdate += () =>
        {
            if (Stamina > 0)
            {
                var closestBoid = GetClosestBoid();

                if (closestBoid != null)
                    Chase(closestBoid);
                else
                    SendInputToFSM(HunterInputs.PATROL);
            }
            else
                SendInputToFSM(HunterInputs.REST);
        };





        REST.OnEnter += x =>
        {
            _render.material.color = Color.gray;
        };

        REST.OnUpdate += () =>
        {
            if (Stamina <= maxStamina)
            {
                Rest();
                return;
            }
            
            var closestBoid = GetClosestBoid();

            if (closestBoid != null)
                SendInputToFSM(HunterInputs.CHASE);
            else
                SendInputToFSM(HunterInputs.PATROL);
        };

        REST.OnExit += x =>
        {
            ChangeStamina(maxStamina);
        };





        _fsm = new EventFSM<HunterInputs>(PATROL);

        //_fsm.AddState(HunterStates.Patrol, new PatrolState(this, _render));
        //_fsm.AddState(HunterStates.Chase, new ChaseState(this, _render));
        //_fsm.AddState(HunterStates.Rest, new RestState(this, _render));
        //_fsm.ChangeState(HunterStates.Patrol);
    }

    private void SendInputToFSM(HunterInputs inp)
    {
        _fsm.SendInput(inp);
    }

    protected override void Update()
    {
        transform.position = BoidManager.Instance.UpdateBoundPosition(transform.position);

        _fsm.Update();
        base.Update();
    }
    #endregion

    #region STATES METHODS
    public void Patrol()
    {
        CalculateWaypoints();
        AddForce(Seek(waypoints[_currentWaypoint].position));

        MovingBehaviour();

        _stamina--;
    }

    public void Chase(LOB_Boid closest)
    {
        if(closest != null)
            AddForce(Pursuit(closest));

        MovingBehaviour();

        _stamina -= 3;
    }

    public void Rest()
    {
        _stamina += 5;
    }

    public void ChangeStamina(float newStamina)
    {
        _stamina = newStamina;
    }
    #endregion

    #region AUXILIAR STATES METHODS
    void MovingBehaviour()
    {
        Move();

        var oaForce = AvoidObstacles();
        var force = oaForce == Vector3.zero ? CalculateSteering(Velocity.normalized * _maxSpeed) : oaForce;

        AddForce(force * oAWeight);
    }

    void CalculateWaypoints()
    {
        if (Vector3.Distance(transform.position, waypoints[_currentWaypoint].position) <= waypointDistance)
            _currentWaypoint++;

        if (_currentWaypoint >= waypoints.Length)
            _currentWaypoint = 0;
    }

    //public bool CheckForCloseBoids(HashSet<Boid> boids, float radius)
    //{
    //    bool b = false;

    //    foreach (var item in boids)
    //    {
    //        if (Vector3.Distance(item.transform.position, transform.position) < radius)
    //        {
    //            b = true;
    //            break;
    //        }
    //    }

    //    return b;
    //}

    public LOB_Boid GetClosestBoid()
    {
        var targetPos = Vector3.zero;
        LOB_Boid target = default;

        foreach (var gridEnt in _queries.Query())
        {
            float itemDist = Vector3.Distance(gridEnt.transform.position, transform.position);
            float desiredDist = Vector3.Distance(targetPos, transform.position);

            if (targetPos == Vector3.zero || itemDist < desiredDist)
            {
                targetPos = gridEnt.transform.position;

                var boid = gridEnt.GetComponent<LOB_Boid>();

                if (boid != null)
                    target = boid;
            }
        }

        if (target == null || target == default)
            return null;
        else
            return target;
    }
    #endregion

    #region GIZMOS
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(waypoints[_currentWaypoint].position, waypointDistance);

        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);







        Gizmos.color = Color.red;

        Gizmos.DrawLine((transform.position + transform.up / viewRange), (transform.position + transform.up / viewRange) + Velocity.normalized * viewRadius);
        Gizmos.DrawLine((transform.position - transform.up / viewRange), (transform.position - transform.up / viewRange) + Velocity.normalized * viewRadius);

        Gizmos.DrawLine((transform.position - transform.up / viewRange) + Velocity.normalized * viewRadius, (transform.position + transform.up / viewRange) + Velocity.normalized * viewRadius);
    }
    #endregion
}

public enum HunterStates
{
    Patrol,
    Chase,
    Rest
}