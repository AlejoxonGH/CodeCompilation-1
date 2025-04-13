using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOB_SteeringAgent : GridEntity
{
    public Vector3 Velocity { get; private set; }

    [SerializeField] protected float _maxSpeed;
    [Range(0, 0.1f)][SerializeField] protected float _maxForce;

    [SerializeField] float _arriveRadius;

    public float viewRadius;
    public float viewRange;

    public LayerMask obstacleMask;
    public float oAWeight;

    public LOB_SteeringAgent pursuitTarget;
    public Transform target;
    public float pursuitRadius;

    #region BASIC STEERING MOVING METHODS (Move, AddForce & CalculateSteering)
    protected void Move()
    {
        transform.position += Velocity * Time.deltaTime;
        transform.right = Velocity;
    }

    protected void AddForce(Vector3 force)
    {
        Velocity = Vector3.ClampMagnitude(Velocity + force, _maxSpeed);
    }

    protected Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude(desired - Velocity, _maxForce);
    }
    #endregion

    #region OBSTACLE AVOIDANCE
    protected Vector3 AvoidObstacles()
    {
        var desired = Vector3.zero;

        if (Physics.Raycast(transform.position + transform.forward / viewRange, Velocity, viewRadius, obstacleMask) && Physics.Raycast(transform.position - transform.forward / viewRange, Velocity, viewRadius, obstacleMask))
            desired = -transform.forward;
        else if (Physics.Raycast(transform.position + transform.forward / viewRange, Velocity, viewRadius, obstacleMask))
            desired = -transform.forward;
        else if (Physics.Raycast(transform.position - transform.forward / viewRange, Velocity, viewRadius, obstacleMask))
            desired = transform.forward;
        else return Vector3.zero;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }
    #endregion

    #region SEEK & FLEE
    protected Vector3 Seek(Vector3 pos)
    {
        return CalculateSteering((pos - transform.position).normalized * _maxSpeed);
    }

    protected Vector3 Seek(Vector3 pos, float speed)
    {
        return CalculateSteering((pos - transform.position).normalized * speed);
    }

    protected Vector3 Flee(Vector3 pos)
    {
        return -Seek(pos);
    }

    protected Vector3 Flee(Vector3 pos, float speed)
    {
        return -Seek(pos, speed);
    }
    #endregion

    #region PURSUIT & EVADE
    protected Vector3 Pursuit(LOB_SteeringAgent agent)
    {
        //Pos del target + su velocity * Time
        var futurePos = agent.transform.position + agent.Velocity; /* esto puede multiplicarse por Time.deltaTime y un valor escalar */

        //Forma2 si estoy en un rango determinado, persigo al agente directamente (sin posicion futura)
        if (Vector3.Distance(transform.position, agent.transform.position) <= agent.Velocity.magnitude * 0.5)//valor a preguntar varia
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

        return Seek(futurePos);
    }

    protected Vector3 Evade(LOB_SteeringAgent agent)
    {
        return -Pursuit(agent);
    }
    #endregion

    #region ARRIVE
    protected Vector3 Arrive(Vector3 targetPos)
    {
        var dist = Vector3.Distance(transform.position, targetPos);
        if (dist > _arriveRadius)
            return Seek(targetPos);

        //desired
        var desired = (targetPos - transform.position).normalized;
        desired *= _maxSpeed * (dist / _arriveRadius);

        //float speed = _maxSpeed - (_maxSpeed * (dist/_arriveRadius)); reverse Arrive

        //Steering
        var steering = Vector3.ClampMagnitude(desired - Velocity, _maxForce * Time.deltaTime);

        return steering;
    }
    #endregion


    public void ChangeVelocity(float v)
    {
        Velocity /= v;
    }
}
