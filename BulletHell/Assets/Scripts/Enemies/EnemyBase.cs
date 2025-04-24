using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float Health;
    [SerializeField] public float Damage;
    [SerializeField] public float currentHealth;
    private Coroutine burnCoroutine;

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
}
