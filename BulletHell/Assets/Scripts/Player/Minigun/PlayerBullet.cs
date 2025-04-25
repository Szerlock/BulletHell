using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;
    private bool burnOn = false;
    public float burnDuration = 5f;
    public float burnTickDamage = 1f;
    public float burnTickInterval = 1f;
    private float critChance;
    private float critMultiplier;
    public void InitBullet(bool burnActive, float critChance, float critMultiplier)
    {
        burnOn = burnActive;
        this.critChance = critChance;
        this.critMultiplier = critMultiplier;
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
