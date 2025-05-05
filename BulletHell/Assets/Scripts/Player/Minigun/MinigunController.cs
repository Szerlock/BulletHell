using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigunController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private CharacterController3D characterController;
    [SerializeField] private float bulletLifetime;
    [SerializeField] private Camera cam;
    [SerializeField] private float bulletSpeed;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator barrelAnimator;


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

    [Header("Tracking Bullets Augment")]
    public bool trackingUnlocked = false;

    [Header("Triple Shot")]
    [SerializeField] private List<Transform> extraFirePoints; 
    public bool tripleShotUnlocked = false;

    [Header("Distance Damage Increase Augment")]
    public bool distanceUnlocked = false;
    
    [Header("Short Distance Damage Increase Augment")]
    public bool shortDistanceUnlocked = false;

    [Header("CameraShake")]
    public float shakeDuration;
    public float shakeMagnitude;
    public float shakeFrequency;
    [SerializeField] private GameObject shakeCam;

    [Header("Rotate Minigun object")]
    public Vector3 offset;
    public Vector3 minigunOffset;
    public GameObject minigun;
    public Transform followMinigun;

    void Start()
    {
        baseFireRate = fireRate;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        animator.SetBool("isMoving", playerMovement.IsMoving());

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
                    animator.SetBool("isShooting", true);
                    barrelAnimator.SetBool("isShooting", true);
                    playerMovement.isAiming = true;
                    fireCooldown = 1f / fireRate;
                }
            }
            else
            {
                playerMovement.isHovering = false;
                animator.SetBool("isShooting", false);
                barrelAnimator.SetBool("isShooting", false);
                playerMovement.isAiming = false;
            }
        }
        FollowMinigun();
        RotateMinigunTowardsCamera();
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
            //if (playerMovement.IsMoving())
            //{
            //    playerMovement.isHovering = false;
            //    return;
            //}
            if (!playerMovement.IsMoving())
            {
                playerMovement.isHovering = true;
            }
        }

        SpawnBullet(firePoint, direction);
        shakeCam.GetComponent<CameraShake>().Shake(shakeDuration, shakeMagnitude, shakeFrequency);

        if (tripleShotUnlocked)
        {
            foreach (var point in extraFirePoints)
            {
                SpawnBullet(point, direction);
            }
        }

        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            playerMovement.transform.rotation = Quaternion.Slerp(playerMovement.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void SpawnBullet(Transform point, Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, point.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * bulletSpeed;

        bullet.GetComponent<PlayerBullet>().InitBullet(
            bulletsBurnUnlocked,
            characterController.critChance,
            characterController.critMultiplier,
            trackingUnlocked,
            bulletSpeed,
            distanceUnlocked,
            shortDistanceUnlocked,
            bulletLifetime
        );
    }

    bool GetCrosshairWorldDirection(out Vector3 direction)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        direction = Vector3.forward;


        Ray ray = cam.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 rawDirection = (hit.point - firePoint.position).normalized;

            if (Vector3.Dot(rawDirection, cam.transform.forward) < 0.1f)
            {
                direction = cam.transform.forward;
            }
            else
            {
                direction = rawDirection;
            }

            return true;
        }

        direction = cam.transform.forward;
        return true;

    }

    private void FollowMinigun()
    {
        transform.position = followMinigun.position;
        transform.rotation = followMinigun.rotation;
        transform.localScale = followMinigun.localScale;
    }

    private void RotateMinigunTowardsCamera()
    {
        Vector3 lookDirection = playerMovement.cameraTransform.forward;
        lookDirection.y = 0f;

        if (lookDirection.magnitude >= 0.1f)
        {
            Quaternion currentRotation = minigun.transform.rotation;

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            Quaternion offsetRotation = Quaternion.Euler(minigunOffset);
            targetRotation *= offsetRotation;

            Vector3 euler = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 10f).eulerAngles;
            euler.x = currentRotation.eulerAngles.x;

            if(playerMovement.isGrounded)
            {
                float pitch = playerMovement.cameraTransform.localEulerAngles.x;

                Quaternion characterRotation = Quaternion.Euler(pitch, minigun.transform.eulerAngles.y, 0f);
                minigun.transform.rotation = Quaternion.Slerp(minigun.transform.rotation, characterRotation, Time.deltaTime * 10f);
            }
            else 
                minigun.transform.rotation = Quaternion.Euler(euler);
        }
    }
}
