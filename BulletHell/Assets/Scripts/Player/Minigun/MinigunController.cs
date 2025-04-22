using UnityEngine;

public class MinigunController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera camera;

    [SerializeField] private float fireRate = 10f;
    [SerializeField] private float aimSpeed = 5f;

    // Only use if i add more enemies
    [SerializeField] private LayerMask enemyLayer;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Vector3 shootDirection;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetCrosshairWorldDirection(out shootDirection))
                {
                    Shoot(shootDirection);
                    fireCooldown = 1f / fireRate;
                }
            }
        }
    }

    void Shoot(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * 50f;

        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            playerMovement.transform.rotation = Quaternion.Slerp(playerMovement.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    bool GetCrosshairWorldDirection(out Vector3 direction)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = camera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            direction = (hit.point - firePoint.position).normalized;
            return true;
        }

        direction = ray.direction;
        return true;
    }
}
