using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LOB_Boid : LOB_SteeringAgent
{
    public float boidViewRadius;
    public float separationRadius;
    public float deathRadius;

    //Weighted Beahaviors
    [Header("Weights")]
    [Range(0f, 5f)] public float separationWeight = 1;
    [Range(0f, 3f)] public float alignmentWeight = 1;
    [Range(0f, 3f)] public float cohesionWeight = 1;

    [Range(0f, 10f)] public float foodSeekWeight = 1;

    [SerializeField] Queries _evadeQuery;
    [SerializeField] Queries _closerBoidsQuery;
    [SerializeField] Queries _separationQuery;
    [SerializeField] Queries _deathQuery;

    #region MONOBEHAVIOUR METHODS (Start, Update, etc.)
    void Start()
    {
        Vector3 vectorRandom = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        AddForce(vectorRandom.normalized * _maxSpeed);

        BoidManager.Instance.AddBoid(this);

        _evadeQuery.radius = pursuitRadius;
        _closerBoidsQuery.radius = boidViewRadius;
        _separationQuery.radius = separationRadius;
        _deathQuery.radius = deathRadius;

        _evadeQuery.targetGrid = _closerBoidsQuery.targetGrid = _separationQuery.targetGrid = _deathQuery.targetGrid = SpatialGrid.Instance;
    }

    protected override void Update()
    {
        if (_deathQuery.Query().OfType<WaypointAgent>().Any())
        {
            Destroy(gameObject);
            return;
        }
        
        transform.position = BoidManager.Instance.UpdateBoundPosition(transform.position);

        if (_evadeQuery.Query().OfType<WaypointAgent>().Any())
            AddForce(Evade(pursuitTarget));
        else
        {
            Flocking();

            if (target != null)
            {
                if (_evadeQuery.Query().OfType<Food>().Any())
                    AddForce(Seek(target.position * foodSeekWeight));
            }
        }

        Move();

        Vector3 oaForce = AvoidObstacles();
        Vector3 force = oaForce == Vector3.zero ? CalculateSteering(Velocity.normalized * _maxSpeed) : oaForce;
        AddForce(force * oAWeight);


        base.Update();
        UpdateGridPos();
    }
    #endregion

    #region Flocking
    void Flocking()
    {
        AddForce(Separation() * separationWeight);
        AddForce(Alignment() * alignmentWeight);
        AddForce(Cohesion() * cohesionWeight);
    }

    Vector3 Separation()
    {
        var desired = Vector3.zero;
        var closeBoids = _separationQuery.Query().OfType<LOB_Boid>();

        if (closeBoids.Any())
        {
            foreach (var boid in closeBoids)
            {
                if (boid == this) continue;

                var dist = boid.transform.position - transform.position;
                desired += dist;
            }
        }

        if (desired == Vector3.zero) return desired;

        desired *= -1;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    Vector3 Alignment()
    {
        var desired = Vector3.zero;
        var count = 0;

        var closeBoids = _closerBoidsQuery.Query().OfType<LOB_Boid>();

        //Deseo ir hacia el promedio de la dir en donde se dirijen mis compañeros cercanos
        if (closeBoids.Any())
        {
            foreach (var boid in closeBoids)
            {
                if (boid == this) continue;

                desired += boid.Velocity;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        //dividir por la cant
        desired /= count;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }
    
    Vector3 Cohesion()
    {
        Vector3 desiredPos = Vector3.zero;
        int count = 0;

        var closeBoids = _closerBoidsQuery.Query().OfType<LOB_Boid>();

        //Conseguir el promedio de las posiciones de los boids cercanos
        //promedio = sumar los elementos / cantidad
        if (closeBoids.Any())
        {
            foreach (var boid in closeBoids)
            {
                if (boid != this) count++;

                desiredPos += boid.transform.position;
            }
        }

        if (count == 0) return Vector3.zero;

        desiredPos /= (count /*+ 1*/);

        return Seek(desiredPos);
    }
    #endregion

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == 7)
    //    {
    //        //gameObject.SetActive(false);
    //        Destroy(gameObject);
    //    }
    //}

    private void OnDisable()
    {
        if (BoidManager.Instance == null || SpatialGrid.Instance == null) return;

        BoidManager.Instance.CallRespawnCoroutine();
        SpatialGrid.Instance.Unsubscribe(this);
        BoidManager.Instance.RemoveBoid(this);
    }

    #region GIZMOS
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, boidViewRadius);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, separationRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine((transform.position + transform.forward / viewRange), (transform.position + transform.forward / viewRange) + Velocity.normalized * viewRadius);
        Gizmos.DrawLine((transform.position - transform.forward / viewRange), (transform.position - transform.forward / viewRange) + Velocity.normalized * viewRadius);
        Gizmos.DrawLine((transform.position - transform.forward / viewRange) + Velocity.normalized * viewRadius, (transform.position + transform.forward / viewRange) + Velocity.normalized * viewRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);

        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, deathRadius);
    }
    #endregion
}
