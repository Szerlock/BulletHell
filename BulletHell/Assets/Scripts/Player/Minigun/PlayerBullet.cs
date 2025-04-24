using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;
    private bool burnOn = false;
    public float burnDuration = 5f;
    public float burnTickDamage = 1f;
    public float burnTickInterval = 1f;

    public void InitBullet(bool burnActive)
    {
        burnOn = burnActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                {
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
        }
    }
}
