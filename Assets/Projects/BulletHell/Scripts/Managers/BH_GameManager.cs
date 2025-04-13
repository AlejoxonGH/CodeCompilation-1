using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BH_GameManager : MonoBehaviour
{
    public static BH_GameManager Instance { get; private set; }

    [HideInInspector] public int coinCount = 0;

    [SerializeField] Transform _topLeft;
    [SerializeField] Transform _botRight;

    [SerializeField] float _xLimit;

    [SerializeField] float _cycleTime = 0.2f;
    public List<BH_Enemy> enemies;

    Func<BH_Enemy, bool> _enemiesWithLife, _enemiesOutOfBounds;
    Func<BH_Enemy, IEnumerable<PickUp>> _getPickUpColl;
    Func<BH_Enemy, float> _orderByPositionInX, _orderByPositionInY;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _enemiesWithLife = (x => x.Life > 0);
        _getPickUpColl = (x => x.GenerateLootInCollection(x.GetPercentage()));
        _orderByPositionInX = (x => x.transform.position.x);
        _orderByPositionInY = (y => y.transform.position.y);
        _enemiesOutOfBounds = (x => x.transform.position.x <= _xLimit);

        StartCoroutine(OrderCycle());
    }

    public void Explode()
    {
        Debug.Log("EXPLODED SCREEN");
        
        DeleteAllEnemies();
    }

    void DeleteAllEnemies()
    {
        var loot = GetAllPickUps(enemies);
        StartCoroutine(DeleteEnemies(enemies.Count));

        var coins = loot.OfType<CoinUp>().ToArray();
        var notCoins = loot.Select(x => x.NotType<CoinUp>()).Where(x => x != null).ToArray();

        StartCoroutine(SpawnLoot(coins, coins.Length, true));
        StartCoroutine(SpawnLoot(notCoins, notCoins.Length, false));
    }

    public void AddCoins(int newCoins)
    {
        coinCount += newCoins;
        BH_UIManager.Instance.UpdateUI();
    }

    public void UpdateEnemyList(IEnumerable<BH_Enemy> newEnemies)
    {
        enemies = ConcatUpdated(enemies, newEnemies);
    }

    IEnumerator DeleteEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Destroy(enemies[i].gameObject);
            yield return null;
        }

        enemies.Clear();
    }

    IEnumerator SpawnLoot(PickUp[] loot, int count, bool areCoins)
    {
        float x;
        float y;

        for (int i = 0; i < count; i++)
        {
            x = UnityEngine.Random.Range(_topLeft.position.x, _botRight.position.x);
            y = UnityEngine.Random.Range(_botRight.position.y, _topLeft.position.y);

            if (areCoins)
                Instantiate(loot[i], new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0, -30, 90)));
            else
                Instantiate(loot[i], new Vector3(x, y, 0), Quaternion.identity);

            yield return null;
        }
    }

    IEnumerator OrderCycle()
    {
        while (true)
        {
            if (CheckIfNull(enemies))
            {
                enemies = OrderedList(enemies);
                var missedEnemiesCount = GetMissedEnemiesCount(enemies);

                if (missedEnemiesCount > 0)
                {
                    if (missedEnemiesCount > 1)
                    {
                        var missedEnemies = TakeMissedEnemies(enemies, missedEnemiesCount);

                        for (int i = 0; i < missedEnemies.Length; i++)
                            missedEnemies[i].OnDeath();
                    }
                    else
                        FirstMissedEnemy(enemies).OnDeath();
                }
            }

            yield return new WaitForSeconds(_cycleTime);
        }
    }

    //IA2-LINQ
    IEnumerable<PickUp> GetAllPickUps(List<BH_Enemy> es)
    {
        return es.Where(_enemiesWithLife).SelectMany(_getPickUpColl);
    }

    List<BH_Enemy> ConcatUpdated(List<BH_Enemy> es, IEnumerable<BH_Enemy> nes)
    {
        return es.Concat(nes).ToList();
    }

    bool CheckIfNull(List<BH_Enemy> es)
    {
        return es.Any();
    }

    List<BH_Enemy> OrderedList(List<BH_Enemy> es)
    {
        return es.OrderBy(_orderByPositionInX).ThenBy(_orderByPositionInY).ToList();
    }

    int GetMissedEnemiesCount(List<BH_Enemy> es)
    {
        return es.Count(_enemiesOutOfBounds);
    }

    BH_Enemy[] TakeMissedEnemies(List<BH_Enemy> es, int count)
    {
        return es.Take(count).ToArray();
    }

    BH_Enemy FirstMissedEnemy(List<BH_Enemy> es)
    {
        return es.First();
    }
    //IA2-LINQ

    public void DeleteEnemy(BH_Enemy e)
    {
        if (e != null && enemies.Contains(e))
            enemies.Remove(e);
    }
}