using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
    public static ObstacleFactory Instance { get; private set; }

    public Transform obstacleSpawnPoint;
    public Transform birdSpawnPoint;

    [SerializeField] List<Obstacle> _obstaclePrefabList;
    [SerializeField] int _obstacleStock = 5;

    [SerializeField] float _minTimeSpawn;
    [SerializeField] float _maxTimeSpawn;
    bool _canSpawn = false;

    ObjectPool<Obstacle> _pool;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _pool = new ObjectPool<Obstacle>(ObstacleCreator, Obstacle.TurnOn, Obstacle.TurnOff, _obstacleStock);

        StartCoroutine(SpawnCycle());
    }

    private void Update()
    {
        if (_canSpawn)
        {
            GetObject();
            StartCoroutine(SpawnCycle());
        }
    }

    Obstacle ObstacleCreator()
    {
        return Instantiate(_obstaclePrefabList[Random.Range(0, _obstaclePrefabList.Count)], transform);
    }

    public Obstacle GetObject()
    {
        return _pool.GetObject();
    }

    public void ReturnObstacle(Obstacle o)
    {
        _pool.ReturnObject(o);
    }

    IEnumerator SpawnCycle()
    {
        _canSpawn = false;
        yield return new WaitUntil(() => this.enabled);
        yield return new WaitForSeconds(Random.Range(_minTimeSpawn, _maxTimeSpawn));
        _canSpawn = true;
    }
}
