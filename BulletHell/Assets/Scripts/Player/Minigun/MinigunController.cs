using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigunController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera camera;
    [SerializeField] private CharacterController3D characterController;

    [SerializeField] private float fireRate = 10f;

    // Only use if i add more enemies
    [SerializeField] private LayerMask enemyLayer;

    private float fireCooldown = 0f;

    [Header("RampingUp Variables")]
    [SerializeField] private float rampUpTimeToMax = 3f; 
    [SerializeField] private float rampUpBonus = 5f;
    public bool rampingUPUnlocked = false;
    
    [Header("Burning Bullets Variables")]
    public bool bulletsBurnUnlocked = false;

    private float rampTimer = 0f;
    private float baseFireRate;
    private Vector3 lastPosition;

    void Start()
    {
        baseFireRate = fireRate;
        lastPosition = transform.position;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (playerMovement.IsMoving())
        {
            rampTimer = 0f;
            fireRate = baseFireRate;
        }

        if (fireCooldown <= 0f)
        {
            Vector3 shootDirection;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetCrosshairWorldDirection(out shootDirection))
                {
                    if (rampingUPUnlocked)
                        IncreaseAttackSpeed();

                    Shoot(shootDirection);
                    fireCooldown = 1f / fireRate;
                }
            }
            else
            {
                playerMovement.isHovering = false;
            }
        }
    }

    private void IncreaseAttackSpeed()
    {
        if (!playerMovement.IsMoving())
        {
            rampTimer += Time.deltaTime;
        }

        float t = Mathf.Clamp01(rampTimer / rampUpTimeToMax);
        fireRate = baseFireRate + Mathf.Lerp(0, rampUpBonus, t);
    }

    void Shoot(Vector3 direction)
    {
        if (!playerMovement.isGrounded)
        {
            if (playerMovement.IsMoving())
            {
                playerMovement.isHovering = false;
                return;
            }
            else
            {
                playerMovement.isHovering = true;
            }
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * 50f;

        if (bulletsBurnUnlocked)
        {
            bullet.GetComponent<PlayerBullet>().InitBullet(
                bulletsBurnUnlocked,
                characterController.critChance,
                characterController.critMultiplier,
                characterController.trackingUnlocked
            );
        }

        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            playerMovement.transform.rotation = Quaternion.Slerp(playerMovement.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    bool GetCrosshairWorldDirection(out Vector3 direction)
    {
        //Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        //Ray ray = camera.ScreenPointToRay(screenCenter);

        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    direction = (hit.point - firePoint.position).normalized;
        //    return true;
        //}

        //direction = ray.direction;

        //return true;
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = camera.ScreenPointToRay(screenCenter);

        // Distance from firePoint to ray hit, but clamp so it's always forward
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 rawDirection = (hit.point - firePoint.position).normalized;

            // If looking down and hit is below firePoint, fall back to camera forward
            if (Vector3.Dot(rawDirection, camera.transform.forward) < 0.1f)
            {
                direction = camera.transform.forward;
            }
            else
            {
                direction = rawDirection;
            }

            return true;
        }

        direction = camera.transform.forward;
        return true;

    }
}
