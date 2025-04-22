using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> allBosses;
    private Transform currentBoss;
    public static BossManager Instance { get; private set; }

    private int currentBossIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentBoss = allBosses[currentBossIndex].transform;
    }

    public void NextBoss()
    {
        if (currentBossIndex + 1 < allBosses.Count)
        {
            currentBossIndex++;
            currentBoss = allBosses[currentBossIndex].transform;
        }
    }

    public Transform GetCurrentBoss()
    {
        return currentBoss;
    }
}
