using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform _upperPoint;
    [SerializeField] Transform _downerPoint;
    Vector3 _upperPos, _downerPos;

    [SerializeField] BH_Enemy _enemyPrefab;

    [SerializeField] int[] _enemyLifePresets;
    [SerializeField] int[] _enemyDamagePresets;
    [SerializeField] Color[] _enemyColorPresets;
    [SerializeField] float[] _enemyFireRatePresets;

    [SerializeField] float _minWaitTime;
    [SerializeField] float _maxWaitTime;

    [SerializeField] int _minEnemyCount;
    [SerializeField] int _maxEnemyCount;

    //IA2-LINQ
    List<Tuple<int, int, Color, float>> _enemyTypes;
    //IA2-LINQ

    Func<int, int, Tuple<int, int>> _zipLifeWithDmg;
    Func<Tuple<int, int>, Color, Tuple<int, int, Color>> _zipLifeAndDmgWithColor;
    Func<Tuple<int, int, Color>, float, Tuple<int, int, Color, float>> _zipAll;

    private void Start()
    {
        if (!(_enemyLifePresets.Length == _enemyDamagePresets.Length && _enemyDamagePresets.Length == _enemyColorPresets.Length && _enemyColorPresets.Length == _enemyFireRatePresets.Length))
            Debug.LogError("SOBRAN VARIANTES DE ENEMIGOS");

        _zipLifeWithDmg = ((l, d) => Tuple.Create(l, d));
        _zipLifeAndDmgWithColor = ((ld, c) => Tuple.Create(ld.Item1, ld.Item2, c));
        _zipAll = ((ldc, fr) => Tuple.Create(ldc.Item1, ldc.Item2, ldc.Item3, fr));

        _enemyTypes = SetTupleList(_enemyLifePresets, _enemyDamagePresets, _enemyColorPresets, _enemyFireRatePresets);

        _upperPos = _upperPoint.position;
        _downerPos = _downerPoint.position;

        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        while (true)
        {
            var q = UnityEngine.Random.Range(_minEnemyCount, _maxEnemyCount);
            var enemyPack = q.SpawnEnemyPack(_enemyTypes, _enemyPrefab, _upperPos, _downerPos);

            yield return new WaitForSeconds(UnityEngine.Random.Range(_minWaitTime, _maxWaitTime));

            BH_GameManager.Instance.UpdateEnemyList(enemyPack);
        }
    }

    //IA2-LINQ
    List<Tuple<int, int, Color, float>> SetTupleList(int[] lifes, int[] dmgs, Color[] colors, float[] fireRates)
    {
        return lifes
            .Zip(dmgs, _zipLifeWithDmg)
            .Zip(colors, _zipLifeAndDmgWithColor)
            .Zip(fireRates, _zipAll)
            .ToList();
    }
}

public static class Extensions
{
    public static IEnumerable<BH_Enemy> SpawnEnemyPack(this int enemyCount, List<Tuple<int, int, Color, float>> types, BH_Enemy e, Vector3 upper, Vector3 downer)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            BH_Enemy enemy = UnityEngine.Object.Instantiate(e, new Vector3(upper.x, UnityEngine.Random.Range(downer.y, upper.y), 0), Quaternion.identity);
            var type = types[UnityEngine.Random.Range(0, types.Count)];
            enemy.Initialize(type.Item1, type.Item2, type.Item3, type.Item4);
            yield return enemy;
        }
    }

    public static PickUp NotType<T>(this PickUp p)
    {
        if (p is T)
            return null;
        else
            return p;
    }
}
//IA2-LINQ