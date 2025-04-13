using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : Obstacle
{
    [SerializeField] Rigidbody2D _rgbd;
    
    [SerializeField] float _speed;

    IMovement _movementType;
    [SerializeField] SpriteRenderer _renderer;

    private void Start()
    {
        int i = Random.Range(1, 9);

        if (i == 8)
        {
            _movementType = new SinMovement(_rgbd, transform, _speed);
            _renderer.color = Color.cyan;
        }
        else
            _movementType = new LinearMovement(_rgbd, transform, _speed);
    }

    private void FixedUpdate()
    {
        _movementType.Move();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        RunnerPlayer p = collision.GetComponent<RunnerPlayer>();

        if (p != null)
            p.Die();
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
        _rgbd.constraints = RigidbodyConstraints2D.None;
        _rgbd.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
