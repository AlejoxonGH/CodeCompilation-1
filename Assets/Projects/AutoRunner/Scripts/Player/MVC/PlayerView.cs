using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView
{
    Rigidbody2D _rgbd;
    Animator _anim;

    GameObject _shield;
    GameObject _gun;

    public PlayerView(Rigidbody2D rgbd, Animator anim, GameObject shield, GameObject gun)
    {
        _rgbd = rgbd;
        _anim = anim;

        _shield = shield;
        _gun = gun;
    }

    public void ViewUpdate()
    {
        _anim.SetFloat("yVelocity", _rgbd.velocity.y);
    }

    public void ChangeAnimationTo(string a)
    {
        _anim.Play(a);
    }

    public void TurnOnShield()
    {
        _shield.SetActive(true);
    }

    public void TurnOffShield()
    {
        _shield.SetActive(false);
    }

    public void TurnOnGun()
    {
        _gun.SetActive(true);
    }

    public void TurnOffGun()
    {
        _gun.SetActive(false);
    }
}
