using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage;
    private bool burnOn = false;
    public float burnDuration = 5f;
    public float burnTickDamage = 1f;
    public float burnTickInterval = 1f;
    private float critChance;
    private float critMultiplier;
    private float lifeTimer;

    [Header("Tracking Augment Settings")]
    private EnemyBase targetEnemy = null;
    [SerializeField] private float trackingSpeed;
    private bool trackingUnlocked;
    private Rigidbody rb;
    private float bulletSpeed;
    [SerializeField] private float trackingDistance;

    [Header("Distance Damage Increase Augment")]
    [SerializeField] private bool distanceDamageAugment = false;
    [SerializeField] private float distanceMultiplier = 0.1f;
    [SerializeField] private float maxEffectiveDistance = 30f;
    private Transform playerTransform;

    [Header("Short Distance Damage Increase")]
    [SerializeField] private bool shortRangeAugment = false;
    [SerializeField] private float maxBonusDistance = 10f;
    [SerializeField] private float maxBonusMultiplier = 4f;

    public void InitBullet(bool burnActive, float critChance, float critMultiplier, bool tracking, float speed, bool dtAugment, bool shortUnlocked, float bulletLifetime)
    {
        burnOn = burnActive;
        trackingUnlocked = tracking;
        this.critChance = critChance;
        this.critMultiplier = critMultiplier;
        damage = GameManager.Instance.Player.damage;
        rb = GetComponent<Rigidbody>();
        bulletSpeed = speed;
        distanceDamageAugment = dtAugment;
        shortRangeAugment = shortUnlocked;
        playerTransform = GameManager.Instance.Player.transform;
        lifeTimer = shortRangeAugment ? bulletLifetime / 2f : bulletLifetime;
        if (shortRangeAugment)
        {
            damage *= 2f;
        }
    }

    private void Start()
    {
        if (trackingUnlocked)
            //targetEnemy = GameManager.Instance.FindClosestEnemy();
            targetEnemy = BossManager.Instance.currentBoss;
    }

    private void Update()
    {
        HandleBulletLifeSpan();

        if (trackingUnlocked)
        {
            TrackEnemy();
        }
    }

    private void HandleBulletLifeSpan()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            DeactivateBullet();
        }
    }

    private void TrackEnemy()
    {
        if (targetEnemy != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToTarget < trackingDistance) 
            {
                Vector3 directionToTarget = (targetEnemy.transform.position - transform.position).normalized;

                rb.linearVelocity = directionToTarget * bulletSpeed;

                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * trackingSpeed);
            }
            else
            {
                rb.linearVelocity = transform.forward * bulletSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                {

                    bool isCrit = Random.value < critChance;
                    float finalDamage = damage;

                    finalDamage = CalculateLongDistance(enemy, finalDamage);

                    finalDamage = CalculateShortDistance(enemy, finalDamage);

                    if (isCrit)
                    {
                        finalDamage *= critMultiplier;
                        Debug.Log("CRIT! Damage dealt: " + finalDamage);
                    }

                    enemy.TakeDamage(finalDamage);

                    if (burnOn)
                    {
                        BurnEffect effect = GlobalBurnStats.GetBurnEffect();
                        enemy.ApplyBurn(effect.tickDamage, effect.tickInterval, effect.duration);
                    }
                }
                break;

            case "EnemyBullet":
                break;
            case "Player":
                break;
            case "OilZone":
                if (!burnOn)
                    return;
                OilZone oilZone = other.GetComponent<OilZone>();
                if (oilZone != null)
                {
                    oilZone.Ignite();
                }
                break;
            case "MiniBallerina":
                BallerinaUnit mini = other.GetComponent<BallerinaUnit>();
                if (mini)
                    mini.TakeDamage(damage);
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }

    private float CalculateShortDistance(EnemyBase enemy, float finalDamage)
    {
        if (shortRangeAugment && playerTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, enemy.transform.position);

            if (distance < maxBonusDistance)
            {
                float t = Mathf.Clamp01(1f - (distance / maxBonusDistance)); 
                float bonusMultiplier = Mathf.Lerp(1f, maxBonusMultiplier, t); 

                finalDamage *= bonusMultiplier;
            }
        }

        return finalDamage;
    }

    private float CalculateLongDistance(EnemyBase enemy, float finalDamage)
    {
        if (distanceDamageAugment && playerTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, enemy.transform.position);
            distance = Mathf.Min(distance, maxEffectiveDistance);

            float extraDamage = distance * distanceMultiplier;
            finalDamage += extraDamage;
        }

        return finalDamage;
    }

    private void DeactivateBullet()
    {
        Destroy(gameObject);
    }
}
