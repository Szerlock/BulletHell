using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController3D : MonoBehaviour
{

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private FlashingEffect flashingEffect;

    [Header("PlayerStats")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] public float damage;
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
    public bool isInvincibleUnlocked = false;

    [Header("Invincibility Settings")]
    [SerializeField] private float iframeDuration = 0.5f;
    private bool isIFrameActive = false;
    private float iframeTimer = 0f;

    [Header("Dragon Augment Settings")]
    public GameObject fireDragonPrefab;
    public GameObject bombDragonPrefab;
    public GameObject healingDragonPrefab;
    public GameObject shadowDragonPrefab;
    public List<Transform> spawnPositions;
    private List<bool> positionOccupied = new List<bool> { false, false, false, false };

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isInvincibleUnlocked)
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
                StartInvincibility();
            }
        }
        else
        {
            invincibleDurationTimer += Time.deltaTime;

            if (invincibleDurationTimer >= 3f)
            {
                EndInvincibility();
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
        //flashingEffect.Flash(iframeDuration);
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
        //EnemyBase closest = null;
        //float minDistance = Mathf.Infinity;

        //foreach (EnemyBase enemy in GameManager.Instance.AllEnemies)
        //{
        //    if (enemy == null) continue;

        //    float dist = Vector3.Distance(transform.position, enemy.transform.position);
        //    if (dist < minDistance)
        //    {
        //        minDistance = dist;
        //        closest = enemy;
        //    }
        //}

        //return closest;

        return BossManager.Instance.currentBoss;
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

    [ContextMenu("SpawnBombDragon")]
    internal void SpawnBombDragon()
    {
        int index = GetAvailablePositionIndex();
        GameObject bombDragon = Instantiate(bombDragonPrefab, spawnPositions[index].position, Quaternion.identity);
        bombDragon.GetComponent<FollowCharDragon>().SetTarget(spawnPositions[index]);
        positionOccupied[index] = true;
    }

    [ContextMenu("SpawnHealingDragon")]
    internal void SpawnHealingDragon()
    {
        int index = GetAvailablePositionIndex();
        GameObject healingDragon = Instantiate(healingDragonPrefab, spawnPositions[index].position, Quaternion.identity);
        healingDragon.GetComponent<FollowCharDragon>().SetTarget(spawnPositions[index]);
        positionOccupied[index] = true;
    }

    [ContextMenu("SpawnShadowDragon")]
    internal void SpawnShadowDragon()
    {
        int index = GetAvailablePositionIndex();
        GameObject shadowDragon = Instantiate(shadowDragonPrefab, spawnPositions[index].position, Quaternion.identity);
        shadowDragon.GetComponent<FollowCharDragon>().SetTarget(spawnPositions[index]);
        positionOccupied[index] = true;
    }

    [ContextMenu("SpawnFireDragon")]
    internal void SpawnFireDragon()
    {
        int index = GetAvailablePositionIndex();
        GameObject fireDragon = Instantiate(fireDragonPrefab, spawnPositions[index].position, Quaternion.identity);
        fireDragon.GetComponent<FollowCharDragon>().SetTarget(spawnPositions[index]);
        positionOccupied[index] = true;
    }

    private int GetAvailablePositionIndex()
    {
        for (int i = 0; i < positionOccupied.Count; i++)
        {
            if (!positionOccupied[i])
                return i;
        }

        Debug.LogWarning("All dragon slots are occupied!");
        return -1;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Player = this;
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibleCooldownTimer = 0f;
        invincibleDurationTimer = 0f;

        Debug.Log("You are now INVINCIBLE!");
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        invincibleDurationTimer = 0f;

        Debug.Log("Invincibility ended.");
    }

    public void SetMaxHealth(float multiplier)
    {
        float oldMaxHealth = maxHealth;
        maxHealth *= multiplier;

        Heal(maxHealth - oldMaxHealth);
    }

    public void IncreasePower(float multiplier)
    {
        damage *= multiplier;
    }

    public void IncreaseMoveSpeed(float multiplier)
    {
        movement.moveSpeed *= multiplier;
    }

    public void IncreaseSize(Vector3 localScale)
    {
        gameObject.transform.localScale += localScale;
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
    }

    public void PushPlayer()
    {
        TakeDamage(1);
        StartCoroutine(movement.ApplyPushBackwards());
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        BossBase boss = hit.gameObject.GetComponent<BossBase>();
        if (boss != null)
        {
            PushPlayer();
        }
        else if (hit.gameObject.tag == "MiniBallerina")
            PushPlayer();
    }
}

