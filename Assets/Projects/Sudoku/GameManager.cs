using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<IStuned> _allStuned;

    public void UpdateStuns(IEnumerable<IStuned> stunedPeople)
    {
        foreach (var stuned in stunedPeople)
        {
            stuned.SetSeconds(Random.Range(11, 22));
            _allStuned.Add(stuned);
        }

        for (int i = 0; i < _allStuned.Count; i++)
        {
            _allStuned[i].SetSeconds(_allStuned[i].GetSeconds() - 1);

            if (_allStuned[i].GetSeconds() <= 0)
            {
                _allStuned[i].Ready();
                _allStuned.RemoveAt(i);
            }
        }
    }
}

public interface IStuned
{
    void SetSeconds(int seconds);
    int GetSeconds();
    void Ready();
}