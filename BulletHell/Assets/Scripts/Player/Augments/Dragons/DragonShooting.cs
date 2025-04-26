using UnityEngine;
using static DragonType;

public class DragonShooting : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileSpeed; 
    [SerializeField] private ProjectileType projectileType;
    private float shootTimer;

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown)
        {
            shootTimer = 0f;
            ShootAtNearestEnemy();
        }
    }

    private void ShootAtNearestEnemy()
    {
        EnemyBase target = FindClosestEnemy();
        if (target == null) return;

        Vector3 dir = (target.transform.position - shootPoint.position).normalized;

        GameObject projGO = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(dir));
        DragonProjectile proj = projGO.GetComponent<DragonProjectile>();
        proj.Init(dir, projectileType, projectileDamage, projectileSpeed);
    }

    private EnemyBase FindClosestEnemy()
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

    public void UpgradeDragons(float multiplier)
    {
        projectileDamage *= multiplier;
        shootCooldown = Mathf.Max(0.1f, shootCooldown * multiplier);
    }
}
