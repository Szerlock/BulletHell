using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<EnemyBase> AllEnemies = new List<EnemyBase>();
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

    public void AddEnemy(EnemyBase enemy)
    {

            AllEnemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyBase enemy)
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

    public EnemyBase FindClosestEnemy()
    {
        EnemyBase closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in GameManager.Instance.AllEnemies)
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

    [ContextMenu("Clear List")]
    public void ClearEnemies()
    {
        AllEnemies.Clear();
    }
}
