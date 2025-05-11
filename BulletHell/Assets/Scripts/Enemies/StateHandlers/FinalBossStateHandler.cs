using NUnit.Framework;
using System;
using UnityEngine;

public class FinalBossStateHandler : StateHandler
{

    public override void Init(BossBase bossInstance)
    {
        boss = bossInstance;
        fireCooldown = boss.fireCooldown;
    }

    public override void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            if (!GameManager.Instance.isInCinematic)
            {
                if (boss)
                    if (boss.isInitialized)
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
                        }
                    }
            }
        }
    }

    public override void HandleWaiting()
    {
        if (boss.isPlayingPose)
            return; 
        fireCooldown -= Time.deltaTime;

        if(boss.isUnstable)
            boss.isUnstable = false;

        if (fireCooldown <= 0f)
        {
            boss.currentState = BossBase.State.Moving;
        }
    }

    public override void HandleAttacking()
    {
        if (boss.SecondPhase)
        {
            fireCooldown = boss.unstableFireCooldown;
            boss.isUnstable = true;
        }
        else
            fireCooldown = boss.fireRate;

        boss.bulletSpawner.StartFiring();

        if (boss.bulletSpawner.AttackFinished() && !boss.isPlayingPose)
        {
            if(boss.isConjuring)
                boss.isConjuring = false;
            boss.currentState = BossBase.State.Waiting;
            boss.bulletSpawner.ResetAttack();
        }
    }


    public override void HandleMoving()
    {
        boss.MoveToTarget();
        if (!boss.isMoving)
        {
            boss.currentState = BossBase.State.Attacking;
            boss.hasTarget = false;
        }
    }
}
