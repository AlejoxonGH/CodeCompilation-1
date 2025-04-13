using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMovement : IMovement
{
    Rigidbody2D _rgbd;
    Transform _transform;
    float _speed;

    public SinMovement(Rigidbody2D rgbd, Transform transform, float speed)
    {
        _rgbd = rgbd;
        _transform = transform;
        _speed = speed;
    }

    public void Move()
    {
        Vector3 SinY = _transform.up * Mathf.Sin(6 * Time.time) * 0.08f;

        _rgbd.MovePosition( new Vector2( _transform.position.x - _speed * Time.fixedDeltaTime, _transform.position.y + SinY.y));
    }
}
