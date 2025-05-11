using UnityEngine;

public class BallerinaUnit : MonoBehaviour
{
    private MiniBallerinaBoss boss;
    private Vector3 targetPosition;
    [SerializeField] private float baseRotateSpeed = 360f;
    [SerializeField] public ChangeMaterial changeMaterial;
    public BulletSpawner bulletSpawner;
    public float maxHealth;
    public float currentHealth;
    private float rotateSpeed;
    private float unstableRotateSpeed;

    public void Init(MiniBallerinaBoss bossController, float health)
    {
        boss = bossController;
        bulletSpawner.boss = bossController;
        maxHealth = health;
        currentHealth = maxHealth;

        rotateSpeed = baseRotateSpeed;
        float flip = Random.value > 0.5f ? 1f : -1f;
        float speedMultiplier = Random.Range(1.2f, 1.8f);
        unstableRotateSpeed = rotateSpeed * flip * speedMultiplier;

        GameManager.Instance.AddEnemy(transform);
    }

    public void MoveToPosition(Vector3 position)    
    {
        targetPosition = position + Vector3.up * 7.5f;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            if (!GameManager.Instance.isInCinematic)
            {
                if (!boss) return;

                if (boss.isInitialized)
                {
                    if (!boss.isUnstable)
                    {
                        rotateSpeed = baseRotateSpeed;
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 10f);
                    }
                    else
                    {
                        rotateSpeed = unstableRotateSpeed;
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, boss.speed * Time.deltaTime);
                    }

                }
                else if (targetPosition != Vector3.zero && !boss.isInitialized)
                {
                    rotateSpeed = baseRotateSpeed;
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 30f);
                }
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }
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
