using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    Transform _transform;
    
    Rigidbody2D _rgbd;

    Transform _bulletInstancePos;

    public float speed;

    float _jumpForce;

    PlayerView _pV;

    public PlayerModel(Transform transform, Rigidbody2D rgbd, Transform bulletInstancePos, float initialSpeed, float jumpForce, PlayerView pV)
    {
        _transform = transform;

        _rgbd = rgbd;

        _bulletInstancePos = bulletInstancePos;

        speed = initialSpeed;

        _jumpForce = jumpForce;

        _pV = pV;
    }

    public void ModelFixedUpdate()
    {
        Move();
    }

    void Move()
    {
        _rgbd.velocity = new Vector2(speed, _rgbd.velocity.y);
    }

    public void Jump()
    {
        if (RunnerPlayer.Instance.isGrounded)
        {
            _rgbd.AddForce(Vector3.up * _jumpForce, ForceMode2D.Impulse);

            if (RunnerPlayer.Instance.CanBeHit)
            {
                _pV.ChangeAnimationTo("FrogJump");
            }
        }
    }

    public void Shoot()
    {
        RunnerBullet b = BulletFactory.Instance.GetObject();
        b.transform.position = _bulletInstancePos.position;
        RunnerPlayer.Instance.StartShootingCooldown();
    }
}
