using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RvB_SteeringAgent : MonoBehaviour
{
    [Space(40)]
    [Header("RIGIDBODY")] //////////////////////////////////////////////////////// RIGIDBODY //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected Rigidbody _rb;

    #region STEERING SPEED VARIABLES
    [Space(40)]
    [Header("SPEED VARIABLES")] //////////////////////////////////////////////////////// SPEED VARIABLES //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected float _maxSpeed;
    [SerializeField][Range(0, 0.1f)] protected float _maxForce;

    public Vector3 Velocity { get; protected set; }
    #endregion

    #region STEERING RADIUS VARIABLES
    [Space(40)]
    [Header("RADIUS VARIABLES")] //////////////////////////////////////////////////////// RADIUS VARIABLES //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected float _arriveRadius;
    [SerializeField] protected float _pursuitRadius;

    protected RvB_SteeringAgent _pursuitTarget = null;
    #endregion

    #region OBSTACLE AVOIDANCE VARIABLES
    [Space(40)]
    [Header("OBSTACLE AVOIDANCE")] //////////////////////////////////////////////////////// OBSTACLE AVOIDANCE //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    public float oAViewRadius;
    public float oAViewRange;

    public LayerMask obstacleMask;
    public float oAWeight;
    #endregion



    #region BASIC STEERING MOVING METHODS (Move, AddForce & CalculateSteering)

    protected void FixedMove() => FixedMove(Velocity);
    protected void FixedMove(Vector3 lookTarget) 
    {
        _rb.MovePosition(transform.position + Velocity * Time.deltaTime);
        transform.right = lookTarget;
    }

    protected void AddForce(Vector3 force) => Velocity = Vector3.ClampMagnitude(Velocity + force, _maxSpeed);
    protected Vector3 CalculateSteering(Vector3 desired) => Vector3.ClampMagnitude(desired - Velocity, _maxForce);
    public void ChangeVelocity(float velocityDivider) => Velocity /= velocityDivider;
    #endregion

    #region OBSTACLE AVOIDANCE
    protected Vector3 AvoidObstacles()
    {
        var desired = Vector3.zero;

        if (Physics.Raycast(transform.position + transform.up / oAViewRange, Velocity, oAViewRadius, obstacleMask))
            desired = -transform.up;
        else if (Physics.Raycast(transform.position - transform.up / oAViewRange, Velocity, oAViewRadius, obstacleMask))
            desired = transform.up;
        else return Vector3.zero;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }
    #endregion

    #region SEEK & FLEE
    protected Vector3 Seek(Vector3 targetPos) => Seek(targetPos, _maxSpeed);
    protected Vector3 Seek(Vector3 targetPos, float speed) => CalculateSteering((targetPos - transform.position).normalized * speed);

    protected Vector3 Flee(Vector3 pursuerPos) => Flee(pursuerPos, _maxSpeed);
    protected Vector3 Flee(Vector3 pursuerPos, float speed) => -Seek(pursuerPos, speed);
    #endregion

    #region PURSUIT & EVADE
    protected Vector3 Pursuit(RvB_SteeringAgent agent) => Pursuit(agent, _maxSpeed);
    protected Vector3 Pursuit(RvB_SteeringAgent agent, float speed)
    {
        var futurePos = agent.transform.position + agent.Velocity;
        
        if (Vector3.Distance(transform.position, agent.transform.position) <= agent.Velocity.magnitude * 0.5)
        {
            Debug.DrawLine(transform.position, agent.transform.position, Color.green);
            return Seek(agent.transform.position);
        }

        if (Vector3.Distance(transform.position, futurePos) < agent.Velocity.magnitude)
        {
            Debug.DrawLine(transform.position, transform.position + (agent.transform.position - transform.position) / 1.5f, Color.blue);
            return Arrive(transform.position + (agent.transform.position - transform.position) / 1.5f);
        }

        Debug.DrawLine(transform.position, futurePos, Color.red);
        return Seek(futurePos, speed);
    }

    protected Vector3 Evade(RvB_SteeringAgent agent) => -Pursuit(agent);
    #endregion

    #region ARRIVE
    protected Vector3 Arrive(Vector3 targetPos)
    {
        var dist = Vector3.Distance(transform.position, targetPos);

        if (dist > _arriveRadius)
            return Seek(targetPos);

        var desired = (targetPos - transform.position).normalized;
        desired *= _maxSpeed * (dist / _arriveRadius);
        var steering = Vector3.ClampMagnitude(desired - Velocity, _maxForce * Time.deltaTime);

        return steering;
    }
    #endregion

    protected void SADrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _arriveRadius);

        Gizmos.color = new Color(1, 0.47f, 0.01f, 1); // ORANGE
        Gizmos.DrawWireSphere(transform.position, _pursuitRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + transform.up / oAViewRange, (transform.position + transform.up / oAViewRange) + (transform.right.normalized * oAViewRadius));
        Gizmos.DrawLine(transform.position - transform.up / oAViewRange, (transform.position - transform.up / oAViewRange) + (transform.right.normalized * oAViewRadius));
    }
}