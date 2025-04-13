using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    [SerializeField] Transform _playerCamPos;

    private void Update()
    {
        if (_playerCamPos != null)
        {
            transform.position = new Vector3(_playerCamPos.position.x, transform.position.y, transform.position.z);
        }
    }
}
