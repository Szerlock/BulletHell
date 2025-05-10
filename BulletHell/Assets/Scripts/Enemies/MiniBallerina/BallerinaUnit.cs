using UnityEngine;

public class BallerinaUnit : MonoBehaviour
{
    private MiniBallerinaBoss boss;
    private Vector3 targetPosition;
    [SerializeField] private float rotateSpeed = 360f;
    [SerializeField] public ChangeMaterial changeMaterial;
    public BulletSpawner bulletSpawner;
    public float maxHealth;
    public float currentHealth;

    public void Init(MiniBallerinaBoss bossController, float health)
    {
        boss = bossController;
        bulletSpawner.boss = bossController;
        maxHealth = health;
        currentHealth = maxHealth;
        GameManager.Instance.AddEnemy(transform);
    }

    public void MoveToPosition(Vector3 position)    
    {
        targetPosition = position + Vector3.up * 7.5f;
    }

    private void Update()
    {
        if (boss)
            if (boss.isInitialized)
            {
                if (!boss.isUnstable)
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 10f);
                else
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, boss.speed * Time.deltaTime);

                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }
            else if(targetPosition != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 10f);
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }
    }

    public void TakeDamage(float amount)
    {
        if (!boss.isInitialized) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            BallerinaDead();
        }
        boss.TakeDamage(amount);
    }

    public bool HasReachedTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance < 0.1f;
    }

    public void BallerinaDead()
    {
        boss.RemoveBallerina(this);
        GameManager.Instance.RemoveEnemy(transform);
        Destroy(gameObject);
    }
}
