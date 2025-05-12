using System;
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

    [Header("Visual")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material dangerMaterial;
    [SerializeField] private Material unstableMat;
    [SerializeField] private float dangerDistance = 5f;

    [SerializeField] private Renderer rend;
    private bool isInDangerState = false;

    public float DangerDistance => dangerDistance;
    public bool IsInDangerState => isInDangerState;


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

    private IEnumerator LifeTimer()
    {
        float timerLife = lifeTime;
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            if (UIManager.Instance.backgroundUp)
            {
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"[BulletManager] Bullet {name} expired after {timerLife} seconds.");
        Disable();
    }

    public void ChangeMat(bool unstableBullets)
    {
        if (unstableBullets)
            rend.material = unstableMat;
        else
            rend.material = normalMaterial;
    }

    public void SetDangerState(bool danger)
    {
        if (rend == null || danger == isInDangerState) return;

        isInDangerState = danger;
        rend.material = danger ? dangerMaterial : normalMaterial;
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
            case "ArenaBorder":
                Disable();
                break;
        }
    }

    public void Disable()
    {
        //Debug.Log("Disable");
        BulletPool.Instance.ReturnBullet(gameObject);
    }

    void OnEnable()
    {
        BulletManager.Instance.Register(this);
        //lifeCoroutine = StartCoroutine(LifeTimer());
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
}
