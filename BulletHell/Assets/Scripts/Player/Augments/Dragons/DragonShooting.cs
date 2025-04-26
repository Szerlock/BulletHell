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
        EnemyBase target = GameManager.Instance.FindClosestEnemy();
        if (target == null) return;

        Vector3 dir = (target.transform.position - shootPoint.position).normalized;

        GameObject projGO = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(dir));
        DragonProjectile proj = projGO.GetComponent<DragonProjectile>();
        proj.Init(dir, projectileType, projectileDamage, projectileSpeed);
    }

    public void UpgradeDragons(float multiplier)
    {
        projectileDamage *= multiplier;
        shootCooldown = Mathf.Max(0.1f, shootCooldown * multiplier);
    }
}
