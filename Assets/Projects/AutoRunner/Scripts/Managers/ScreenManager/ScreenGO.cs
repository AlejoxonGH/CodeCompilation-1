using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenGO : MonoBehaviour, IScreen
{
    Dictionary<Behaviour, bool> _before;
    [SerializeField] Transform _root;

    RunnerPlayer _p;
    Bird _b;
    Slime _s;

    Vector2 _playerVelocityStorage;
    List<Vector2> _birdVelocityStorage;
    List<Vector2> _slimeVelocityStorage;

    private void Start()
    {
        _before = new Dictionary<Behaviour, bool>();
        _playerVelocityStorage = Vector2.zero;
        _birdVelocityStorage = new List<Vector2>();
        _slimeVelocityStorage = new List<Vector2>();
    }

    public void Activate()
    {        
        if (!_root.gameObject.activeSelf) _root.gameObject.SetActive(true);

        if (_before == null || _before.Count <= 0) return;

        foreach (var keyValue in _before)
        {
            keyValue.Key.enabled = keyValue.Value;

            if (keyValue.Key.gameObject.TryGetComponent<RunnerPlayer>(out _p))
            {
                _p.TurnOnRigidbody(_playerVelocityStorage);
            }

            if (keyValue.Key.gameObject.TryGetComponent<Bird>(out _b))
            {
                _b.TurnOnRigidbody(_birdVelocityStorage[0]);
                _birdVelocityStorage.RemoveAt(0);
            }

            if (keyValue.Key.gameObject.TryGetComponent<Slime>(out _s))
            {
                _s.TurnOnRigidbody(_slimeVelocityStorage[0]);
                _slimeVelocityStorage.RemoveAt(0);
            }
        }

        _playerVelocityStorage = Vector2.zero;
        _birdVelocityStorage.Clear();
        _slimeVelocityStorage.Clear();
        _before.Clear();
    }

    public void Deactivate()
    {
        foreach (var behaviour in _root.GetComponentsInChildren<Behaviour>())
        {
            _before[behaviour] = behaviour.enabled;
            behaviour.enabled = false;

            if (behaviour.gameObject.TryGetComponent<RunnerPlayer>(out _p))
                _playerVelocityStorage = _p.StoreVelocity(_playerVelocityStorage);

            if (behaviour.gameObject.TryGetComponent<Bird>(out _b))
                _birdVelocityStorage = _b.StoreVelocity(_birdVelocityStorage);

            if (behaviour.gameObject.TryGetComponent<Slime>(out _s))
                _slimeVelocityStorage = _s.StoreVelocity(_slimeVelocityStorage);
        }
    }

    public void Free()
    {
        _root.gameObject.SetActive(false);
    }
}
