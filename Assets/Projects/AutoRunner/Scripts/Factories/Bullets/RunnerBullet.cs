using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerBullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rgbd;

    [SerializeField] float _speed;

    [SerializeField] float _lifeTime;
    float _currentLifeTime;

    public static void TurnOn(RunnerBullet b)
    {
        b.Reset();
        b.gameObject.SetActive(true);
    }

    public static void TurnOff(RunnerBullet b)
    {
        b.gameObject.SetActive(false);
    }

    void Update()
    {
        _currentLifeTime -= Time.deltaTime;

        if (_currentLifeTime <= 0)
            BulletFactory.Instance.ReturnBullet(this);
    }

    void FixedUpdate()
    {
        _rgbd.MovePosition(transform.position + transform.right * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlatformCollision>()) return;
        
        BulletFactory.Instance.ReturnBullet(this);
    }

    private void Reset()
    {
        _currentLifeTime = _lifeTime;
    }
}
