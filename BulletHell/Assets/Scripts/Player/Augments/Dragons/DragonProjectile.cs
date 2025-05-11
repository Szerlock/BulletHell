using UnityEngine;
public enum ProjectileType
{
    Bomb,
    Fire,
    Healing,
    Shadow
}

public class DragonProjectile : MonoBehaviour
{
    private Transform direction;
    public ProjectileType projectileType;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float healAmount;
    [SerializeField] private GameObject bombVfx; 

    private float timer = 0f;


    public void Init(Transform dir, ProjectileType type, float dmg, float spd)
    {
        direction = dir;
        projectileType = type;
        speed = spd;
        damage = dmg;
    }

    void Update()
    {
        if (direction == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, direction.position, speed * Time.deltaTime);
       
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
                    Instantiate(bombVfx, transform.position, transform.rotation);
                    Debug.Log("Bomb projectile hit enemy");
                    break;
                case ProjectileType.Fire:
                    enemy.TakeDamage(damage);
                    BurnEffect effect = GlobalBurnStats.GetBurnEffect();
                    enemy.ApplyBurn(effect.tickDamage, effect.tickInterval, effect.duration);
                    break;
                //case ProjectileType.Healing:
                //    Debug.Log("Healing");
                //    break;
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
        if (projectileType == ProjectileType.Healing)
        {
            GameManager.Instance.Player.Heal(healAmount);
        }
    }
}