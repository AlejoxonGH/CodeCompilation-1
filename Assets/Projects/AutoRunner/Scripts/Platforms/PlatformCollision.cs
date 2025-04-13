using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    [SerializeField] Floor _parent;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<RunnerPlayer>())
            _parent.MovePreviousPlatform();
    }
}
