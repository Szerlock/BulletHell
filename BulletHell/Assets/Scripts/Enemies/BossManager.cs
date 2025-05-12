using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private List<BossBase> allBosses;
    public BossBase currentBoss;
    public static BossManager Instance { get; private set; }
    [SerializeField] private int augmentsToPick;
    [SerializeField] public AugmentsPicker augmentManager;


    private int currentBossIndex = 0;

    private void Awake()
    {
       Instance = this;
    }

    public void BossDead()
    {
        StartCoroutine(NextBoss());
    }

    public IEnumerator NextBoss()
    {
        if (currentBossIndex > 0)
            AudioManager.Instance.UnloadBossMusic(currentBossIndex - 1);

        AudioManager.Instance.PreloadBossMusic(currentBossIndex+1);
        yield return new WaitForSeconds(1f);
        //StartCoroutine(augmentManager.StartAugmentPicking(augmentsToPick));
        //yield return new WaitUntil(() => augmentManager.finishedPickingAugment == true);
        //for (int i = 0; i < augmentsToPick; i++)
        //{
        //    SpawnMask.Instance.SpawnMaskAugment();
        //}

        currentBoss = allBosses[currentBossIndex];
        currentBossIndex++;
        currentBoss.Init();
        //yield return null;
    }
}
