using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
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
