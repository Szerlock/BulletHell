using System;
using UnityEngine;

public class CharacterController3D : MonoBehaviour
{

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private FlashingEffect flashingEffect;

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

    [Header("Invincibility Augment")]
    private float invincibleCooldownTimer = 0f;
    private float invincibleDurationTimer = 0f;
    private bool isInvincible = false;

    [Header("Invincibility Settings")]
    [SerializeField] private float iframeDuration = 0.5f;
    private bool isIFrameActive = false;
    private float iframeTimer = 0f;

    void Update()
    {
        HandleInvincibilityTimer();

        HandleIFrames();

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

    private void HandleIFrames()
    {
        if (!isIFrameActive) return;

        iframeTimer += Time.deltaTime;

        if (iframeTimer >= iframeDuration)
        {
            isIFrameActive = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible || isIFrameActive)
            return; 
        flashingEffect.Flash();
        Debug.Log($"Taking {amount} damage. Current health: {currentHealth}");
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ActivateIFrames();
        }
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

    private void ActivateIFrames()
    {
        isIFrameActive = true;
        iframeTimer = 0f;
    }
}
