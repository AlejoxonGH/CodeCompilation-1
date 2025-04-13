using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    [SerializeField] float _lifeTime;
    [SerializeField] float _speed;

    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    void Update()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    public void SetText(string newTextValue, Color color)
    {
        _text.text = newTextValue;
        _text.color = color;
    }
}
