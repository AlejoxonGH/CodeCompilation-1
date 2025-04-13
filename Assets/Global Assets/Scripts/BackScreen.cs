using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackScreen : MonoBehaviour
{
    public static BackScreen Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
