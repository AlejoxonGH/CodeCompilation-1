using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : MonoBehaviour
{
    public static BulletFactory Instance { get; private set; }

    [SerializeField] RunnerBullet _bulletPrefab;
    [SerializeField] int _bulletStock = 5;

    ObjectPool<RunnerBullet> _pool;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _pool = new ObjectPool<RunnerBullet>(BulletCreator, RunnerBullet.TurnOn, RunnerBullet.TurnOff, _bulletStock);
    }

    RunnerBullet BulletCreator()
    {
        return Instantiate(_bulletPrefab, transform);
    }

    public RunnerBullet GetObject()
    {
        return _pool.GetObject();
    }

    public void ReturnBullet(RunnerBullet b)
    {
        _pool.ReturnObject(b);
    }
}
