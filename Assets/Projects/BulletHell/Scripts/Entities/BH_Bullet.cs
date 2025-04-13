using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _speed;
    [SerializeField] float _lifeTime;
    [SerializeField] LayerMask _ignoreLayer;
    
    int _damage = 1;

    private void Start()
    {
        Move();
        Destroy(gameObject, _lifeTime);
    }

    public void Move()
    {
        _rb.velocity = transform.forward * _speed;
    }

    public void SetDamage(int dmg)
    {
        _damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _ignoreLayer) != 0)
            return;

        var hit = other.GetComponent<Hitable>();

        if (hit != null)
            hit.TakeDamage(_damage);

        Destroy(gameObject);
    }
}
