using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPlayer : MonoBehaviour
{
    public static RunnerPlayer Instance { get; private set; }

    [SerializeField] Rigidbody2D _rgbd;

    [SerializeField] Animator _anim;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool HasGun { get; private set; }
    [HideInInspector] public bool CanShoot { get; private set; } = true;
    [HideInInspector] public bool HasShield { get; private set; }
    [HideInInspector] public bool CanBeHit { get; private set; } = true;

    [SerializeField] Transform _bulletInstancePos;

    [SerializeField] float _initialSpeed;

    [SerializeField] float _jumpForce;

    [SerializeField] float _powerUpTime;
    [SerializeField] float _recoverTime;
    [SerializeField] float _shootingCooldownTime;

    [SerializeField] GameObject _shield;
    [SerializeField] GameObject _gun;

    PlayerController _pC;
    PlayerModel _pM;
    PlayerView _pV;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _pV = new PlayerView(_rgbd, _anim, _shield, _gun);
        _pM = new PlayerModel(transform, _rgbd, _bulletInstancePos, _initialSpeed, _jumpForce, _pV);
        _pC = new PlayerController(_pM);
    }

    private void Update()
    {
        _pC.ControllerUpdate();
        _pV.ViewUpdate();
    }

    private void FixedUpdate()
    {
        _pM.ModelFixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
            isGrounded = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
            isGrounded = false;
    }

    public void ChangeSpeed(float newSpeed)
    {
        _pM.speed = newSpeed;

        StartCoroutine(SpeedChangeDuration());
    }

    public void GrabShield()
    {
        HasShield = true;
        _pV.TurnOnShield();

        StartCoroutine(ShieldDuration());
    }

    public void GrabGun()
    {
        HasGun = true;
        _pV.TurnOnGun();

        StartCoroutine(GunDuration());
    }

    public void Die()
    {
        if (!HasShield && CanBeHit)
        {
            RunnerGameManager.Instance.OpenGameOverScreen();
            Destroy(gameObject);
        }
        else if (HasShield)
        {
            HasShield = false;
            _pV.TurnOffShield();
            CanBeHit = false;

            StopCoroutine(ShieldDuration());
            StartCoroutine(Invincibility());
        }
    }

    IEnumerator SpeedChangeDuration()
    {
        yield return new WaitForSeconds(_powerUpTime);
        yield return new WaitUntil(() => this.enabled);
        _pM.speed = _initialSpeed;
    }

    IEnumerator GunDuration()
    {
        yield return new WaitForSeconds(_powerUpTime);
        yield return new WaitUntil(() => this.enabled);

        HasGun = false;
        _pV.TurnOffGun();
    }

    IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(_powerUpTime);
        yield return new WaitUntil(() => this.enabled);

        HasShield = false;
        _pV.TurnOffShield();
        StartCoroutine(Invincibility());
    }

    IEnumerator Invincibility()
    {
        _pV.ChangeAnimationTo("FrogInvincibility");
        yield return new WaitForSeconds(_recoverTime);
        yield return new WaitUntil(() => this.enabled);
        CanBeHit = true;
        _pV.ChangeAnimationTo("FrogRun");
    }

    public void StartShootingCooldown()
    {
        StartCoroutine(ShootingCooldown());
    }

    IEnumerator ShootingCooldown()
    {
        CanShoot = false;
        yield return new WaitForSeconds(_shootingCooldownTime);
        yield return new WaitUntil(() => this.enabled);
        CanShoot = true;
    }

    public Vector2 StoreVelocity(Vector2 velocityStorage)
    {
        velocityStorage = _rgbd.velocity;

        TurnOffRigidbody();

        return velocityStorage;
    }

    public void TurnOffRigidbody()
    {
        _rgbd.velocity = Vector2.zero;

        _rgbd.gravityScale = 0;
        _rgbd.constraints = RigidbodyConstraints2D.FreezeAll;
        _rgbd.bodyType = RigidbodyType2D.Static;
    }

    public void TurnOnRigidbody(Vector3 velocity)
    {
        TurnOnRigidbody();

        _rgbd.velocity = velocity;
    }

    public void TurnOnRigidbody()
    {
        _rgbd.bodyType = RigidbodyType2D.Dynamic;
        _rgbd.gravityScale = 3;
        _rgbd.constraints = RigidbodyConstraints2D.None;
        _rgbd.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}