using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
public enum ProjectileType
{
    Bomb,
    Fire,
    Healing,
    Shadow
}

public class DragonProjectile : MonoBehaviour
{
    private Vector3 direction;
    public ProjectileType projectileType;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float healAmount;

    private float timer = 0f;


    public void Init(Vector3 dir, ProjectileType type, float dmg, float spd)
    {
        direction = dir.normalized;
        projectileType = type;
        speed = spd;
        damage = dmg;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();

            switch (projectileType)
            {
                case ProjectileType.Bomb:
                    enemy.TakeDamage(damage);
                    Debug.Log("Bomb projectile hit enemy");
                    break;
                case ProjectileType.Fire:
                    enemy.TakeDamage(damage);
                    BurnEffect effect = GlobalBurnStats.GetBurnEffect();
                    enemy.ApplyBurn(effect.tickDamage, effect.tickInterval, effect.duration);
                    break;
                case ProjectileType.Healing:
                    GameManager.Instance.Player.Heal(healAmount);
                    Debug.Log("Healing");
                    break;
                case ProjectileType.Shadow:
                    //enemy.ApplySlow();
                    Debug.Log("Shadow projectile hit enemy");
                    break;
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("MiniBallerina"))
        { 
            other.GetComponent<BallerinaUnit>().TakeDamage(damage);
        }
    }
}