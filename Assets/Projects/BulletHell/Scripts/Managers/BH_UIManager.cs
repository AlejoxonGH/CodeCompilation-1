using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BH_UIManager : MonoBehaviour
{
    public static BH_UIManager Instance { get; private set; }

    [SerializeField] BH_Player _pj;
    [SerializeField] BH_GameManager _gm;

    [SerializeField] Image _ExplosionFeed;
    Color _defaultColor;
    [SerializeField] Color _turnedOffColor;
    [SerializeField] TextMeshProUGUI _lifeText;
    [SerializeField] TextMeshProUGUI _coinText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _defaultColor = _ExplosionFeed.color;
        
        _lifeText.text = _pj.MaxLife.ToString();
        _coinText.text = "0";
    }

    public void UpdateUI()
    {
        if (_pj.canExplode)
            _ExplosionFeed.color = _defaultColor;
        else
            _ExplosionFeed.color = _turnedOffColor;
        
        _lifeText.text = _pj.Life.ToString();
        _coinText.text = _gm.coinCount.ToString();
    }
}
