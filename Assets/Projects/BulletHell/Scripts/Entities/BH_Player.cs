using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BH_Player : Hitable, IShooter
{
    public static BH_Player Instance { get; private set; }

    [SerializeField] Rigidbody _rb;
    [SerializeField] Renderer _renderer;
    //[SerializeField] Animator _anim;

    [SerializeField] BH_Bullet _bulletPrefab;
    [SerializeField] Transform _shootPoint;
    [SerializeField] int _bulletDamage;
    [SerializeField] float _shootCooldown;
    [SerializeField] float _explodeCooldown;
    bool _canShoot = true;
    [HideInInspector] public bool canExplode = true;

    [SerializeField] float _speed;
    float _hAxis;
    float _vAxis;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Life = _maxLife;
    }

    private void Update()
    {
        _hAxis = Input.GetAxis("Horizontal");
        _vAxis = Input.GetAxis("Vertical");

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && _canShoot)
            Shoot();

        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKey(KeyCode.Mouse1)) && canExplode)
            Explode();
    }

    private void FixedUpdate()
    {
        if (_hAxis != 0 || _vAxis != 0)
            Move();
    }

    void Move()
    {
        var dir = new Vector3(_hAxis, _vAxis, 0);

        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        _rb.MovePosition(transform.position + ((transform.right * dir.y) + (transform.up * dir.x)) * _speed * Time.fixedDeltaTime);
    }

    public void Shoot()
    {
        var b = Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
        b.SetDamage(_bulletDamage);

        StartCoroutine(ShootCooldown());
    }

    void Explode()
    {
        BH_GameManager.Instance.Explode();
        
        StartCoroutine(ExplodeCooldown());
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        BH_UIManager.Instance.UpdateUI();
        StartCoroutine(RedFlash());
    }

    public override void GetHealed(int heal)
    {
        base.GetHealed(heal);
        BH_UIManager.Instance.UpdateUI();
    }

    public void GetCoins(int coins)
    {
        BH_GameManager.Instance.AddCoins(coins);
    }

    IEnumerator ShootCooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_shootCooldown);
        _canShoot = true;
    }

    IEnumerator ExplodeCooldown()
    {
        canExplode = false;
        BH_UIManager.Instance.UpdateUI();

        yield return new WaitForSeconds(_explodeCooldown);

        canExplode = true;
        BH_UIManager.Instance.UpdateUI();
    }

    IEnumerator RedFlash()
    {
        var color = _renderer.material.color;
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        _renderer.material.color = color;
    }

    public override void OnDeath()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
