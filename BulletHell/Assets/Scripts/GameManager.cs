using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Transform> AllEnemies = new List<Transform>();
    //public BossBase currentBoss;
    public CharacterController3D Player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemy(Transform enemy)
    {
        AllEnemies.Add(enemy);
    }

    public void RemoveEnemy(Transform enemy)
    {
        if (AllEnemies.Contains(enemy))
        {
            AllEnemies.Remove(enemy);
        }
        else
        {
            Debug.LogWarning("Enemy not found in the list.");
        }
    }

    public Transform FindClosestEnemy()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in AllEnemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    //[ContextMenu("Clear List")]
    //public void ClearEnemies()
    //{
    //    AllEnemies.Clear();
    //}
}
