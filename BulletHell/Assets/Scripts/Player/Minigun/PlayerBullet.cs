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

    [Header("Tracking Augment Settings")]
    private EnemyBase targetEnemy = null;
    private float trackingSpeed = 5f;
    private bool trackingUnlocked;
    private Rigidbody rb;

    public void InitBullet(bool burnActive, float critChance, float critMultiplier, bool tracking)
    {
        burnOn = burnActive;
        trackingUnlocked = tracking;
        this.critChance = critChance;
        this.critMultiplier = critMultiplier;
        damage = GameManager.Instance.Player.damage;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        targetEnemy = GameManager.Instance.FindClosestEnemy();
    }

    private void Update()
    {
        if (trackingUnlocked)
        {
            TrackEnemy();
        }
    }

    private void TrackEnemy()
    {
        //if (targetEnemy != null)
        //{
        //    Vector3 directionToTarget = (targetEnemy.transform.position - transform.position).normalized;

        //    rb.linearVelocity = directionToTarget * speed;

        //    Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        //}
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

                    if (isCrit)
                    {
                        finalDamage *= critMultiplier;
                        Debug.Log("CRIT! Damage dealt: " + finalDamage);
                    }

                    enemy.TakeDamage(damage);

                    if (burnOn)
                    {
                        BurnEffect effect = GlobalBurnStats.GetBurnEffect();
                        enemy.ApplyBurn(effect.tickDamage, effect.tickInterval, effect.duration);
                    }
                }
                Destroy(gameObject);
                break;

            case "EnemyBullet":
                Destroy(gameObject);
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
                Destroy(gameObject);
                break;
        }
    }
}
