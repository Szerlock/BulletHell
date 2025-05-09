using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private List<BossBase> allBosses;
    public BossBase currentBoss;
    public static BossManager Instance { get; private set; }
    [SerializeField] private int augmentsToPick;
    [SerializeField] private AugmentsPicker augmentManager;


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
        StartCoroutine(NextBoss());
    }

    public IEnumerator NextBoss()
    {
        augmentManager.finishedPickingAugment = false; 
        StartCoroutine(augmentManager.StartAugmentPicking(augmentsToPick));
        yield return new WaitUntil(() => augmentManager.finishedPickingAugment == true);
        currentBoss = allBosses[currentBossIndex];
        currentBossIndex++;
        currentBoss.Init();
    }
}
