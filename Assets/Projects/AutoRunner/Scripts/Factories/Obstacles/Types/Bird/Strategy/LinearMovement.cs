using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : IMovement
{
    Rigidbody2D _rgbd;
    Transform _transform;
    float _speed;

    public LinearMovement(Rigidbody2D rgbd, Transform transform, float speed)
    {
        _rgbd = rgbd;
        _transform = transform;
        _speed = speed;
    }

    public void Move()
    {
        _rgbd.MovePosition(_transform.position - _transform.right * _speed * Time.fixedDeltaTime);
    }
}
