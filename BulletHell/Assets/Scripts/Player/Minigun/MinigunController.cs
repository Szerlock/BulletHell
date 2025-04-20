using UnityEngine;

public class MinigunController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 10f;
    [SerializeField] private float aimSpeed = 5f;

    private Transform enemyTransform;

    // Only use if i add more enemies
    [SerializeField] private LayerMask enemyLayer;

    private float fireCooldown = 0f;
    private float idleTime = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (playerMovement.IsMoving())
        {
            idleTime = 0f;
            return;
        }

        idleTime += Time.deltaTime;

        enemyTransform = BossManager.Instance.GetCurrentBoss();

        if (enemyTransform == null) return;
        
        if (idleTime >= 0.01f)
        {
            Vector3 direction = (enemyTransform.position - playerMovement.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerMovement.transform.rotation = Quaternion.Slerp(playerMovement.transform.rotation, targetRotation, aimSpeed * Time.deltaTime);
        }

        if (fireCooldown <= 0f && idleTime >= .02f)
        {
            Shoot(enemyTransform);
            fireCooldown = 1f / fireRate;
        }
    }

    void Shoot(Transform target)
    {
        Vector3 direction = (target.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * 50f;
        playerMovement.transform.LookAt(target.position);
    }
}
