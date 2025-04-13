using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_Enemy : Hitable, IShooter
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] Renderer _renderer;

    [SerializeField] Transform _shootPoint;
    [SerializeField] BH_Bullet _bulletPrefab;

    [SerializeField] int _damage;
    [SerializeField] float _fireRate;
    [SerializeField] float _minSpeed;
    [SerializeField] float _maxSpeed;

    [SerializeField] CoinUp _coinPrefab;
    [SerializeField] LifeUp _lifePrefab;

    public void Initialize(int life, int damage, Color color, float fireRate)
    {
        Life = _maxLife = life;
        _damage = damage;
        _renderer.material.color = color;
        _fireRate = fireRate;
        
        InvokeRepeating("Shoot", _fireRate, _fireRate);
        Move(Random.Range(_minSpeed, _maxSpeed));
    }

    public void Move(float speed)
    {
        _rb.velocity = - transform.right * speed;
    }

    public void Shoot()
    {
        var b = Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
        b.SetDamage(_damage);
    }

    public int GetPercentage()
    {
        var n = Random.Range(0, 101);

        if (n <= 60)
            return 1;
        else if (n <= 90)
            return 2;
        else
            return 3;
    }

    //IA2-LINQ
    public IEnumerable<PickUp> GenerateLootInCollection(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Random.Range(0f, 100f) <= 66.6f)
                yield return _coinPrefab;
            else
                yield return _lifePrefab;
        }
    }
    //IA2-LINQ

    void GenerateLoot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Random.Range(0f, 100f) <= 66.6f)
                Instantiate(_coinPrefab, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * 1.125f, Quaternion.Euler(new Vector3(0, -30, 90)));
            else
                Instantiate(_lifePrefab, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * 1.125f, Quaternion.identity);
        }
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        StartCoroutine(RedFlash());
    }

    IEnumerator RedFlash()
    {
        var color = _renderer.material.color;
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        _renderer.material.color = color;
    }

    public override void OnDeath()
    {
        if (Life <= 0)
            GenerateLoot(GetPercentage());
        
        BH_GameManager.Instance.DeleteEnemy(this);
        Destroy(gameObject);
    }
}
