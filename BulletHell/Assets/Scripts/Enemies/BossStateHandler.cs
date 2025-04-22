using NUnit.Framework;
using UnityEngine;

public class BossStateHandler : MonoBehaviour
{
    private BossBase boss;

    private float fireCooldown;

    public void Init(BossBase bossInstance)
    {
        boss = bossInstance;
        fireCooldown = boss.fireCooldown;
    }

    public void Update()
    {
        switch (boss.currentState)
        {
            case BossBase.State.Waiting:
                HandleWaiting();
                break;
            case BossBase.State.Attacking:
                HandleAttacking();
                break;
            case BossBase.State.Moving:
                HandleMoving();
                break;
                // Add more states like Moving, Death, etc.
        }

        LookAtPlayer();
    }

    public void HandleWaiting()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            boss.currentState = BossBase.State.Moving;
        }
    }

    public void HandleAttacking()
    {
        fireCooldown = boss.fireRate;
        boss.bulletSpawner.StartFiring();

        if (boss.bulletSpawner.AttackFinished())
        {
            boss.currentState = BossBase.State.Waiting;
            boss.bulletSpawner.ResetAttack();
        }
    }

    public void HandleMoving()
    {
        boss.MoveToTarget();
        if (!boss.isMoving)
        {
            boss.currentState = BossBase.State.Attacking;
            boss.hasTarget = false;
        }
    }

    private void LookAtPlayer()
    {
        if (boss.player != null)
        {
            Vector3 targetPos = new Vector3(boss.player.position.x, boss.transform.position.y, boss.player.position.z);
            boss.transform.LookAt(targetPos);
        }
    }
}
