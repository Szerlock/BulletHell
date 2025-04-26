using UnityEngine;

public class FollowCharDragon : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private Transform closestEnemy;
    private float findEnemyCooldown = 0.5f;
    private float findEnemyTimer = 0f;

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float dynamicSpeed = followSpeed;

        if (distanceToTarget > 1f)
        {
            dynamicSpeed *= distanceToTarget;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, dynamicSpeed * Time.deltaTime);
        findEnemyTimer -= Time.deltaTime;
        if (findEnemyTimer <= 0f)
        {
            findEnemyTimer = findEnemyCooldown;
            closestEnemy = FindClosestEnemy();
        }

        if (closestEnemy != null)
        {
            Vector3 direction = (closestEnemy.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private Transform FindClosestEnemy()
    {
        EnemyBase closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in GameManager.Instance.AllEnemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest.gameObject.transform;
    }

}
