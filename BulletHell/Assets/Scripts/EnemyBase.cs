using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] public float Health;
    [SerializeField] public float Damage;
    [SerializeField] public float currentHealth;

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

    public abstract void StartPhase();
}
