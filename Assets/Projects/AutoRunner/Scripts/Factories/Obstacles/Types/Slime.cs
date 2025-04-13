using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Obstacle
{
    [SerializeField] Rigidbody2D _rgbd;

    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;

    [SerializeField] float _minTime;
    [SerializeField] float _maxTime;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(JumpTiming());
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        _rgbd.velocity = new Vector2(_speed, _rgbd.velocity.y);
    }

    void Jump()
    {
        _rgbd.AddForce(Vector3.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<RunnerBullet>())
        {
            ObstacleFactory.Instance.ReturnObstacle(this);
        }
        else
        {
            RunnerPlayer p = collision.gameObject.GetComponent<RunnerPlayer>();

            if (p != null)
                p.Die();
        }
    }

    IEnumerator JumpTiming()
    {
        while (true)
        {
            Jump();
            yield return new WaitForSeconds(_minTime);
            yield return new WaitUntil(() => this.enabled);
        }
    }

    public List<Vector2> StoreVelocity(List<Vector2> velocityStorage)
    {
        velocityStorage.Add(_rgbd.velocity);

        TurnOffRigidbody();

        return velocityStorage;
    }

    public void TurnOffRigidbody()
    {
        _rgbd.velocity = Vector2.zero;

        _rgbd.gravityScale = 0;
        _rgbd.constraints = RigidbodyConstraints2D.FreezeAll;
        _rgbd.bodyType = RigidbodyType2D.Static;
    }

    public void TurnOnRigidbody(Vector3 velocity)
    {
        TurnOnRigidbody();

        _rgbd.velocity = velocity;
    }

    public void TurnOnRigidbody()
    {
        _rgbd.bodyType = RigidbodyType2D.Dynamic;
        _rgbd.gravityScale = 0.8f;
        _rgbd.constraints = RigidbodyConstraints2D.None;
        _rgbd.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
