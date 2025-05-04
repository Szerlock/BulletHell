using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float Health;
    [SerializeField] public float Damage;
    [SerializeField] public float currentHealth;
    [SerializeField] public Animator animator;
    private Coroutine burnCoroutine;
    private Coroutine oilBurnCoroutine;

    [Header("Floating Text Range")]
    public float horizontalRange = 0.5f;
    public float upRange;
    public float downRange;


    protected virtual void Start()
    {
        GameManager.Instance.AddEnemy(this);
        currentHealth = Health;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");
        HitNumber(amount);
        if (currentHealth <= 0)
            Die();
    }

    private void HitNumber(float amount)
    {
        Vector3 basePosition = transform.position;
        float heightOffset = GetComponent<Collider>()?.bounds.size.y ?? 2f;

        Vector3 spawnPosition = basePosition + Vector3.up * heightOffset;

        Vector3 randomOffset = new Vector3(
            Random.Range(-horizontalRange, horizontalRange),
            Random.Range(downRange, upRange),
            0f
        );

        spawnPosition += randomOffset;

        HitNumberManager.Instance.ShowHitNumber(spawnPosition, amount, transform);
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }

    public void ApplyBurn(float tickDamage, float interval, float duration)
    {
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine = StartCoroutine(Burn(tickDamage, interval, duration));
    }

    private IEnumerator Burn(float tickDamage, float interval, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            TakeDamage(tickDamage);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
            Debug.Log($"{gameObject.name} took {tickDamage} burn damage. Remaining health: {currentHealth}");
        }

        burnCoroutine = null;
    }

    public void ApplyOilBurn(float tickDamage, float interval, float duration)
    {
        if (oilBurnCoroutine != null)
            StopCoroutine(oilBurnCoroutine);

        oilBurnCoroutine = StartCoroutine(OilBurn(tickDamage, interval, duration));
    }
    private IEnumerator OilBurn(float tickDamage, float interval, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            TakeDamage(tickDamage);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
            Debug.Log($"{gameObject.name} took {tickDamage} burn damage [OIL]. Remaining health: {currentHealth}");
        }

        oilBurnCoroutine = null;
    }

    public virtual void OnEnable()
    {
        //GameManager.Instance.AddEnemy(this);
    }

    public virtual void OnDisable()
    {
        //GameManager.Instance.RemoveEnemy(this);
    }
}
