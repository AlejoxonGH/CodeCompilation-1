using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] GameObject _previousPlatform;
    [SerializeField] Transform _nextPlatformPos;
    
    public void MovePreviousPlatform()
    {
        _previousPlatform.transform.position = _nextPlatformPos.position;
    }
}
