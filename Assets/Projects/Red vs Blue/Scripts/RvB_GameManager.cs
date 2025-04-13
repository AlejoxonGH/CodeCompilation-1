using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RvB_GameManager : MonoBehaviour
{
    public static RvB_GameManager Instance { get; private set; }

    [SerializeField] Transform _redTeamParent;
    [SerializeField] Transform _blueTeamParent;

    public HashSet<RvB_Boid> RedTeamBoids { get; private set; }
    public HashSet<RvB_Boid> BlueTeamBoids { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        RedTeamBoids = new HashSet<RvB_Boid>();
        BlueTeamBoids = new HashSet<RvB_Boid>();
    }

    private void Start()
    {
        foreach (var boid in _redTeamParent.GetComponentsInChildren<RvB_Boid>())
            RedTeamBoids.Add(boid);

        foreach (var boid in _blueTeamParent.GetComponentsInChildren<RvB_Boid>())
            BlueTeamBoids.Add(boid);
    }
}
