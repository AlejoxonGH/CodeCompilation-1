using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    PlayerModel _pM;

    public PlayerController(PlayerModel pM)
    {
        _pM = pM;
    }

    public void ControllerUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _pM.Jump();

        if (Input.GetMouseButtonDown(0) && RunnerPlayer.Instance.HasGun && RunnerPlayer.Instance.CanShoot)
            _pM.Shoot();
    }
}
