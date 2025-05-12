using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMask : MonoBehaviour
{
    public static SpawnMask Instance;
    [SerializeField] private GameObject maskPrefab;
    [SerializeField] private List<Transform> spawnPoint; 
    
    [SerializeField] private List<Transform> arenaMask;

    [SerializeField] private float cooldown;
    public List<GameObject> masks = new List<GameObject>();
    private float timer;
    public bool maskBroken = false;
    private int nextSpawnIndex = 0;
    private int spawnIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

        for (int i = 0; i < 4; i++)
        {
            SpawnMaskInArena();
        }
        timer = cooldown;
    }

    public void SpawnMaskInArena()
    {
        if (masks.Count >= spawnPoint.Count)
            return;

        nextSpawnIndex = (spawnIndex) % spawnPoint.Count;
        masks.Add(Instantiate(maskPrefab, arenaMask[nextSpawnIndex].position + new Vector3(0, 83, 0), Quaternion.identity));
        spawnIndex++;
    }

    public void SpawnMaskAugment()
    {
        if (masks.Count >= spawnPoint.Count)
            return; 
        
        nextSpawnIndex = (spawnIndex) % spawnPoint.Count;

        masks.Add(Instantiate(maskPrefab, spawnPoint[nextSpawnIndex].position + new Vector3(0, 83, 0), Quaternion.identity));
        spawnIndex++;
    }

    void Update()
    {
        if (!GameManager.Instance.isOnTutorial)
        {
            if (BossManager.Instance.currentBoss != null)
            {
                if (BossManager.Instance.currentBoss.isInitialized)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        SpawnMaskAugment();
                        timer = cooldown;
                    }
                }
                foreach (GameObject mask in masks)
                {
                    Vector3 targetPosition = GameManager.Instance.Player.transform.position;
                    Vector3 direction = targetPosition - mask.transform.position;
                    direction.y = 0;
                    mask.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
        else 
        {
            foreach (GameObject mask in masks)
            {
                Vector3 targetPosition = GameManager.Instance.Player.transform.position;
                Vector3 direction = targetPosition - mask.transform.position;
                direction.y = 0;
                mask.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
