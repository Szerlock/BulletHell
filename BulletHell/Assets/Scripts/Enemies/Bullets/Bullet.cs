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
    [SerializeField] private float lifeTime;
    private Coroutine lifeCoroutine;

    private void Start()
    {
        Debug.Log($"[Awake] Bullet prefab default lifetime: {lifeTime}");

    }

    public void InitializeBullet(Vector3 dir, float dmg, float lt)
    {
        direction = dir.normalized;
        damage = dmg;
        lifeTime = lt;

        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
        }
        lifeCoroutine = StartCoroutine(LifeTimer());
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
        Debug.Log("Disable");
        BulletPool.Instance.ReturnBullet(gameObject);
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
