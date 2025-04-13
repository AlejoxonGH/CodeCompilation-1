using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public class RvB_Boid : RvB_SteeringAgent
{
    [Space(40)]
    [Header("RENDERERS")] //////////////////////////////////////////////////////// RENDERERS //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected Renderer _mainRend;
    [SerializeField] protected Renderer _rend;

    #region FSM VARIABLES
    [Space(40)]
    [Header("FSM")] //////////////////////////////////////////////////////// FSM //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    protected EventFSM<BoidStates> _myFsm;

    public enum BoidStates { IDLE, MOVE, ATTACK, FLEE }
    public enum BoidTeam { RED, BLUE }

    [field: SerializeField] public BoidTeam MyBoidTeam { get; protected set; }
    protected void SendInputToFSM(BoidStates inp) => _myFsm.SendInput(inp);
    #endregion

    #region HEALTH VARIABLES 
    [Space(40)]
    [Header("HP")] //////////////////////////////////////////////////////// HP //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected int _highHealth;
    [SerializeField] protected int _lowHealth;

    [field: SerializeField] public int MaxLife { get; protected set; }
    [field: SerializeField] public int Life { get; protected set; }
    
    [SerializeField] protected PvB_Node _healingPoint;
    
    protected bool _canTakeDamage = true;
    protected bool _canGetHealed = true;
    #endregion

    #region SHOOTING VARIABLES
    [Space(40)]
    [Header("SHOOTING")] //////////////////////////////////////////////////////// SHOOTING //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected Transform _shootPoint;
    [SerializeField] protected RvB_Bullet _bulletPrefab;
    protected bool _canShoot = true;
    #endregion

    #region PATHFINDING VARIABLES
    [Space(40)]
    [Header("PATHFINDING")] //////////////////////////////////////////////////////// PATHFINDING //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected GameObject _nodeParent;
    
    protected Pathfinding _pf;
    protected bool _pfTravelingPath = false;
    protected List<Vector3> _path;

    protected PvB_Node start;
    protected PvB_Node goal;
    #endregion

    #region OTHER BOID VARIABLES
    [Space(40)]
    [Header("OTHER")] //////////////////////////////////////////////////////// OTHER //////////////////////////////////////////////////////////////////////////////////////////////
    [Space(15)]

    [SerializeField] protected float _viewRadius;
    [SerializeField][Range(0, 360)] protected float _viewAngle;

    [SerializeField] protected float _touchArriveGoalRadius;
    
    [SerializeField] protected LayerMask _boidLayer;
    [SerializeField] protected LayerMask _obstacleAndWallLayer;

    [SerializeField] protected ScoreText _scoreTextPrefab;

    protected Collider[] _closeColliders;
    #endregion


    #region HP Methods
    public void TakeDamage(int dmg)
    {
        if (!_canTakeDamage) return;

        Life -= dmg;

        if (Life <= 0)
        { OnDeath(); return; }

        if (Life <= _lowHealth)
            SendInputToFSM(BoidStates.FLEE);

        ShowLifeText($"-{dmg}", Color.white);

        StartCoroutine(DamageCooldown());
    }
    public virtual void GetHealed(int heal)
    {
        if (!_canGetHealed) return;

        Life += heal;

        if (Life >= _highHealth)
        {
            if (Life >= MaxLife)
                Life = MaxLife;

            if (_myFsm.Current.Name == "FLEE")
            {
                if (ThereIsACloseEnemyInFOV())
                    SendInputToFSM(BoidStates.ATTACK);
                else
                    SendInputToFSM(BoidStates.IDLE);
            }
        }

        ShowLifeText($"+{heal}", Color.green);

        StartCoroutine(HealCooldown());
    }
    protected virtual void OnDeath()
    {
        if (MyBoidTeam == BoidTeam.RED) RvB_GameManager.Instance.RedTeamBoids.Remove(this);
        else RvB_GameManager.Instance.BlueTeamBoids.Remove(this);

        Destroy(gameObject);
    }
    #endregion

    #region Start & Update
    protected virtual void Start()
    { _pf = new Pathfinding(_obstacleAndWallLayer); Life = MaxLife; }

    protected virtual void Update() => _myFsm.Update();
    #endregion

    #region FIND CLOSE ENEMIES
    protected bool ThereIsACloseEnemyInFOV()
    {
        var enemies = MyBoidTeam == BoidTeam.RED ? RvB_GameManager.Instance.BlueTeamBoids : RvB_GameManager.Instance.RedTeamBoids;

        if (!enemies.Any()) return false;

        var enemiesInFOV = new List<RvB_Boid>();

        foreach (var e in enemies)
        {
            if (transform.position.InFieldOfView(e.transform.position, _obstacleAndWallLayer, _viewRadius, _viewAngle, transform.right))
                enemiesInFOV.Add(e);
        }

        if (enemiesInFOV.Any())
        {
            _pursuitTarget = enemiesInFOV.FindClosestTo(transform.position);
            return true;
        }

        _pursuitTarget = null;
        return false;
    }
    #endregion

    #region SHOOTING
    protected void Shoot()
    {
        var b = Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
        b.SetDamage(10);

        StartCoroutine(ShootCooldown());
    }

    protected IEnumerator ShootCooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(0.75f);
        _canShoot = true;
    }
    #endregion

    #region PATHFINDING !!!!!!
    protected void FindPathToTarget(Vector3 target)
    {
        var myPos = transform.position;

        foreach (var node in _nodeParent.GetComponentsInChildren<PvB_Node>())
        {
            var nodePos = node.gameObject.transform.position;

            if (start == null || goal == null)
            {
                if (start == null && goal == null) { start = node; goal = node; }
                else if (start == null) start = node;
                else goal = node;

                continue;
            }

            var startPos = start.gameObject.transform.position;
            var goalPos = goal.gameObject.transform.position;

            if (Vector3.Distance(nodePos, myPos) < Vector3.Distance(startPos, myPos))
                start = node;

            if (Vector3.Distance(nodePos, target) < Vector3.Distance(goalPos, target))
                goal = node;
        }

        _path = _pf.ThetaStar(start, goal);
        _pfTravelingPath = true;
    }

    protected void TravelPath()
    {
        if (_path != null && _path.Any())
        {
            var desiredPos = _path[0];
            AddForce(Seek(desiredPos));

            if (Vector3.Distance(desiredPos, transform.position) <= _touchArriveGoalRadius)
                _path.RemoveAt(0);
        }
        else if (_path != null && _path.Count <= 0)
            _pfTravelingPath = false;
    }
    #endregion

    #region LIFE COOLDOWNS
    protected IEnumerator DamageCooldown()
    {
        _canTakeDamage = false;
        StartCoroutine(DamageFlash());
        yield return new WaitForSeconds(1);
        _canTakeDamage = true;
    }

    protected IEnumerator DamageFlash()
    {
        var lastColor = _mainRend.material.color;
        while (!_canTakeDamage)
        {
            _mainRend.material.color = Color.black;
            yield return new WaitForSeconds(0.1f);
            _mainRend.material.color = lastColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected IEnumerator HealCooldown()
    {
        _canGetHealed = false;
        yield return new WaitForSeconds(0.5f);
        _canGetHealed = true;
    }
    #endregion

    public void ShowLifeText(string newText, Color color)
    {
        ScoreText st = Instantiate(_scoreTextPrefab, transform.position + Vector3.up * 0.3f + Vector3.back * 2, Quaternion.Euler(0, 0, 0)).GetComponent<ScoreText>();
        st?.SetText(newText, color);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        SADrawGizmos();

        Gizmos.color = Color.green;
        if (_path != null && _path.Any())
        {
            for (int i = 0; i < _path.Count - 1; i++)
                Gizmos.DrawLine(_path[i], _path[i + 1]);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Gizmos.color = Color.blue;
        var dirA = GetDirFromAngle(_viewAngle / 2 + transform.eulerAngles.z);
        var dirB = GetDirFromAngle(-_viewAngle / 2 + transform.eulerAngles.z);
        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * _viewRadius);

        Gizmos.color = new Color(0.25f, 0.05f, 1, 1); ; // PURPLE
        Gizmos.DrawWireSphere(transform.position, _touchArriveGoalRadius);
    }

    Vector3 GetDirFromAngle(float angleInDegrees) => new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
}
