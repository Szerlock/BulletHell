using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float Health = 200f;
    [SerializeField] private float Damage = 10f;
    [SerializeField] private float currentHealth = 200f;

    void Start()
    {
        currentHealth = Health;
    }

    public virtual void TakeDamage(int amount)
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
