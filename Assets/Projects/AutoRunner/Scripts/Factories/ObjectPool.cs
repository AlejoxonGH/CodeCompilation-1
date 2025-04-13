using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T>
{
    Func<T> _factoryMethod;

    public List<T> currentStock;
    bool _isDynamic;

    Action<T> _TurnOnCallback;
    Action<T> _TurnOffCallback;

    public ObjectPool(Func<T> factoryMethod, Action<T> TurnOnCallback, Action<T> TurnOffCallback, int initialAmount, bool isDynamic = true)
    {
        _factoryMethod = factoryMethod;
        _TurnOnCallback = TurnOnCallback;
        _TurnOffCallback = TurnOffCallback;
        _isDynamic = isDynamic;
        currentStock = new List<T>(initialAmount);

        for (int i = 0; i < initialAmount; i++)
        {
            T obj = _factoryMethod();

            _TurnOffCallback(obj);

            currentStock.Add(obj);
        }
    }

    public T GetObject()
    {
        var result = default(T);

        if (currentStock.Count > 0)
        {
            result = currentStock[0];
            currentStock.RemoveAt(0);
        }
        else if (_isDynamic) 
        {
            result = _factoryMethod();
        }

        _TurnOnCallback(result);

        return result;
    }

    public void ReturnObject(T obj)
    {
        _TurnOffCallback(obj);
        currentStock.Add(obj);
    }
}