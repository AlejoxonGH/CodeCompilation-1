using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RvB_Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _speed;
    [SerializeField] float _lifeTime;
    [field: SerializeField] public RvB_Boid.BoidTeam MyBoidTeam { get; private set; }
    [SerializeField] LayerMask _ignoreLayer;

    int _damage = 1;

    void Start()
    { Move(); Destroy(gameObject, _lifeTime); }

    public void Move() => _rb.velocity = transform.up * _speed;

    public void SetDamage(int dmg) => _damage = dmg;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _ignoreLayer) != 0)
            return;

        var boid = other.GetComponent<RvB_Boid>();
        if (boid != null)
        {
            if (boid.MyBoidTeam == MyBoidTeam) return;
            boid.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}