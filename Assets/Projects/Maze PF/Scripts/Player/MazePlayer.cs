using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayer : MonoBehaviour
{
    public float speed;

    [SerializeField] Rigidbody _rb;
    [SerializeField] GameObject _youWin;

    float _horAxis;
    float _verAxis;
    Vector3 _direction;

    void Update()
    {
        _horAxis = Input.GetAxis("Horizontal");
        _verAxis = Input.GetAxis("Vertical");

        _direction = new Vector3(_horAxis, _verAxis, 0);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + ((transform.up * _direction.y) + (transform.right * _direction.x)) * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MazeEnemy>())
            UnityEngine.SceneManagement.SceneManager.LoadScene(4);

        if (other.gameObject.layer == 7)
        {
            _youWin.SetActive(true);
        }
    }
}
