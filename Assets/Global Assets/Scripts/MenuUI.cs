using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] BackScreen _backScreenPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 1)
        {
            if (BackScreen.Instance == null) Instantiate(_backScreenPrefab);
            else Destroy(BackScreen.Instance.gameObject);
        }
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadSceneAsync(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.LogError("QUITTED APPLICATION");
    }
}
