using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUFactory : MonoBehaviour
{
    public static PUFactory Instance { get; private set; }

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    [SerializeField] List<PU> _pUPrefabList;
    [SerializeField] int _pUStock = 5;

    [SerializeField] float _minTimeSpawn;
    [SerializeField] float _maxTimeSpawn;
    bool _canSpawn = false;

    ObjectPool<PU> _pool;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _pool = new ObjectPool<PU>(PUCreator, PU.TurnOn, PU.TurnOff, _pUStock);

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

    PU PUCreator()
    {
        return Instantiate(_pUPrefabList[Random.Range(0, _pUPrefabList.Count)], transform);
    }

    public PU GetObject()
    {
        return _pool.GetObject();
    }

    public void ReturnPU(PU pU)
    {
        _pool.ReturnObject(pU);
    }

    IEnumerator SpawnCycle()
    {
        _canSpawn = false;
        yield return new WaitUntil(() => this.enabled);
        yield return new WaitForSeconds(Random.Range(_minTimeSpawn, _maxTimeSpawn));
        _canSpawn = true;
    }
}
