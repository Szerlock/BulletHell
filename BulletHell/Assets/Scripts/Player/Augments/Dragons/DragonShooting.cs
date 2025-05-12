using UnityEngine;
using static DragonType;

public class DragonShooting : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] public float shootCooldown;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileSpeed; 
    [SerializeField] private ProjectileType projectileType;
    public bool healingDragon;

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
        Transform target = null;

        if (healingDragon)
        {
            target = GameManager.Instance.Player.transform;
        }
        else
        {
            target = GameManager.Instance.FindClosestEnemy();
        }

        if (BossManager.Instance.currentBoss.isHiding)
        {
            target = null;
            return;
        }
        //else
        //{
        //    EnemyBase bossTarget = BossManager.Instance.currentBoss;
        //    if (bossTarget != null)
        //    {
        //        target = bossTarget.transform;
        //    }
        //}

        //EnemyBase target = GameManager.Instance.FindClosestEnemy();
        //EnemyBase target = BossManager.Instance.currentBoss;
        if (target == null) return;

        Vector3 dir = (target.transform.position - shootPoint.position).normalized;

        GameObject projGO = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(dir));
        DragonProjectile proj = projGO.GetComponent<DragonProjectile>();
        proj.Init(target, projectileType, projectileDamage, projectileSpeed);
    }

    public void UpgradeDragons(float multiplier)
    {
        projectileDamage *= multiplier;
        shootCooldown = Mathf.Max(0.1f, shootCooldown * multiplier);
    }
}
