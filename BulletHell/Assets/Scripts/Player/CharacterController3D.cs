using System;
using UnityEngine;

public class CharacterController3D : MonoBehaviour
{

    [SerializeField] private PlayerMovement movement;

    [Header("PlayerStats")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina;
    [SerializeField] public float critChance = .1f;
    [SerializeField] public float critMultiplier = 2f;

    [Header("OilAugmentSettings")]
    [SerializeField] private GameObject OilUrnProyectile;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float fireCooldown = 10f;

    private float fireTimer;
    public bool OilUrnUnlocked = false;

    [Header("Invincibility Settings")]
    private float invincibleCooldownTimer = 0f;
    private float invincibleDurationTimer = 0f;
    private bool isInvincible = false;

    void Update()
    {
        HandleInvincibilityTimer();

        HandleOilUrn();
    }

    private void HandleOilUrn()
    {
        if (!OilUrnUnlocked) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0f;
            EnemyBase target = FindClosestEnemy();

            if (target != null)
            {
                ThrowOilUrnAt(target);
            }
        }
    }

    private void HandleInvincibilityTimer()
    {
        if (!isInvincible)
        {
            invincibleCooldownTimer += Time.deltaTime;

            if (invincibleCooldownTimer >= 60f)
            {
                isInvincible = true;
                invincibleCooldownTimer = 0f;
                invincibleDurationTimer = 0f;

                Debug.Log("You are now INVINCIBLE!");
            }
        }
        else
        {
            invincibleDurationTimer += Time.deltaTime;

            if (invincibleDurationTimer >= 3f)
            {
                isInvincible = false;

                Debug.Log("Invincibility ended.");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // Show death screen or respawn logic
        Debug.Log("You died!");
    }

    private EnemyBase FindClosestEnemy()
    {
        EnemyBase closest = null;
        float minDistance = Mathf.Infinity;

        foreach (EnemyBase enemy in GameManager.Instance.AllEnemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    private void ThrowOilUrnAt(EnemyBase enemy)
    {
        Vector3 dir = (enemy.transform.position - throwPoint.position).normalized;
        GameObject go = Instantiate(OilUrnProyectile, throwPoint.position, Quaternion.LookRotation(dir));
        OilUrnProyectile proj = go.GetComponent<OilUrnProyectile>();


        proj.SetDirection(dir);
    }
}
