using System.Collections;
using System.Data.Common;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    [SerializeField] private float damage;
    public bool isBreakable = false;
    public float health;
    [SerializeField] public float lifeTime;
    private Coroutine lifeCoroutine;


    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void Move(float dt)
    {
        transform.position += direction * speed * dt;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Ground":
                Disable();
                break;
            case "Player":
                CharacterController3D player = GameManager.Instance.Player;
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
                Disable();
                break;
            case "PlayerBullet":
                if (isBreakable)
                { 
                    health -= GameManager.Instance.Player.damage;
                    if (health <= 0)
                    {
                        Disable();
                    }
                }
                break;
        }
    }

    void Disable()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
        Debug.Log("disabling");
    }

    void OnEnable()
    {
        BulletManager.Instance.Register(this);
        lifeCoroutine = StartCoroutine(LifeTimer());
    }

    void OnDisable()
    {
        BulletManager.Instance.Unregister(this);
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Disable();
    }
}
