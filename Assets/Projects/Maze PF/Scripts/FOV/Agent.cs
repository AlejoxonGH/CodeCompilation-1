using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    Renderer _rend;
    void Awake() => _rend = GetComponent<Renderer>();
    public void ChangeColor(Color color) => _rend.material.color = color;
}
