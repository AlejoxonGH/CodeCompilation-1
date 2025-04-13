using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    Dictionary<T, float> _allElements = new Dictionary<T, float>();

    public int Count { get { return _allElements.Count; } }
    public void Enqueue(T elem, float cost)
    {
        if (!_allElements.ContainsKey(elem)) _allElements.Add(elem, cost);
        else _allElements[elem] = cost;
    }

    //Dequeue ? el primer elemento lo remueve y lo devuelve
    public T Dequeue()
    {
        if (_allElements.Count == 0) return default;

        T elem = default;
        //recorrer el diccionario y quedarnos con el que tiene menor costo
        foreach (var item in _allElements)
            elem = elem == null ? item.Key : _allElements[elem] > item.Value ? item.Key : elem;

        _allElements.Remove(elem);
        return elem;
    }
}