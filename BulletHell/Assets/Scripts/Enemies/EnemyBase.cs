using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float Health;
    [SerializeField] public float Damage;
    [SerializeField] public float currentHealth;
    private Coroutine burnCoroutine;
    private Coroutine oilBurnCoroutine;

    void Start()
    {
        currentHealth = Health;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
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
    private void OnEnable()
    {
        //GameManager.Instance.AddEnemy(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveEnemy(this);
    }
}
