using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerGameManager : MonoBehaviour
{
    public static RunnerGameManager Instance { get; private set; }
    
    [SerializeField] ScreenGO _mainGame, _howToPlay, _pause, _gameOver;

    bool _gameStarted = false;
    bool _isPauseActive = false;
    bool _isGameOverActive = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ScreenManager.Instance.Push(_mainGame);
        ScreenManager.Instance.Push(_howToPlay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        else if (!_gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                ScreenManager.Instance.Pop();
                _gameStarted = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) && !_isGameOverActive)
            {
                if (!_isPauseActive)
                {
                    ScreenManager.Instance.Push(_pause);
                    _isPauseActive = true;
                }
                else
                {
                    ScreenManager.Instance.Pop();
                    _isPauseActive = false;
                }
            }
        }
    }

    public void OpenGameOverScreen()
    {
        ScreenManager.Instance.Push(_gameOver);
        _isGameOverActive = true;
    }
}
