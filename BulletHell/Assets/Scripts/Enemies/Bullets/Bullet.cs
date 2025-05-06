using System.Data.Common;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    [SerializeField] private float damage;
    public bool isBreakable = false;
    public float health;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void DestroyAfter(float seconds)
    {
        Invoke(nameof(Disable), seconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Ground":
                Destroy(gameObject);
                break;
            case "Player":
                CharacterController3D player = other.GetComponent<CharacterController3D>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
                break;
            case "PlayerBullet":
                if (isBreakable)
                { 
                    health -= GameManager.Instance.Player.damage;
                    if (health <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
                break;
        }
    }

    void Disable()
    {
        // Send back to pool
        gameObject.SetActive(false);
    }
}
