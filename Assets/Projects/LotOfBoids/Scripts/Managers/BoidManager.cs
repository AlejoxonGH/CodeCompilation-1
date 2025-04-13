using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] Transform _spawnPos;
    [SerializeField] float _timeToRespawn;
    [SerializeField] LOB_SteeringAgent _hunter;
        
    public float halfWidth;
    public float halfHeight;

    public static BoidManager Instance { get; private set; }

    public HashSet<LOB_Boid> allBoids = new HashSet<LOB_Boid>();

    public bool foodSpawned = false;

    public GameObject foodPrefab;
    public GameObject boidPrefab;

    #region MONOBEHAVIOUR METHODS (Start, Update, etc.)
    void Awake()
    {
        if (Instance == null) Instance = this;
        //else Destroy(gameObject);
    }

    private void Update()
    {
        if (!foodSpawned)
        {
            foodSpawned = true;
            
            float x = Random.Range(-halfWidth, halfWidth);
            float z = Random.Range(-halfHeight, halfHeight);

            var foodInstance = Instantiate(foodPrefab, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(90, 0, 0)), SpatialGrid.Instance.gameObject.transform);

            foreach (var item in allBoids)
                item.target = foodInstance.transform;
        }
    }
    #endregion

    public void CallRespawnCoroutine()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_timeToRespawn);
        Instantiate(boidPrefab, _spawnPos.position, Quaternion.Euler(Vector3.zero), SpatialGrid.Instance.gameObject.transform).GetComponent<LOB_Boid>().pursuitTarget = _hunter;
    }

    #region MANAGE BOIDS IN LIST (AddBoid & RemoveBoid)
    public void AddBoid(LOB_Boid b)
    {
        if (allBoids.Contains(b)) return;

        allBoids.Add(b);
    }

    public void RemoveBoid(LOB_Boid b)
    {
        if (allBoids.Contains(b))
        {
            allBoids.Remove(b);
        }
    }
    #endregion

    #region UPDATE BOUND POSITION
    public Vector3 UpdateBoundPosition(Vector3 position)
    {
        if (position.x > halfWidth) position.x = -halfWidth;
        if (position.x < -halfWidth) position.x = halfWidth;
        if (position.z > halfHeight) position.z = -halfHeight;
        if (position.z < -halfHeight) position.z = halfHeight;

        return position;
    }
    #endregion

    #region GIZMOS
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 topLeft = new Vector3(-halfWidth, 0, halfHeight);
        Vector3 topRight = new Vector3(halfWidth, 0, halfHeight);
        Vector3 botRight = new Vector3(halfWidth, 0, -halfHeight);
        Vector3 botLeft = new Vector3(-halfWidth, 0, -halfHeight);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
    #endregion
}
