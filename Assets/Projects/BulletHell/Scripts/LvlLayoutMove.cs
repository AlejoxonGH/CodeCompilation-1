using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlLayoutMove : MonoBehaviour
{
    [SerializeField] float _speed;
    
    void Update()
    {
        transform.position += Vector3.left * _speed * Time.deltaTime;
    }
}
