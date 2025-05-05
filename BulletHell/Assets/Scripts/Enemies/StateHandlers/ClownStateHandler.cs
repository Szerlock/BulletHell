using UnityEngine;

public class ClownStateHandler : StateHandler
{
    private ClownBoss bossType;
    [SerializeField] private float attackCooldown;

    public override void Init(BossBase bossInstance)
    {
        bossType = (ClownBoss)bossInstance;
        attackCooldown = bossType.fireCooldown;
        fireCooldown = attackCooldown;
    }

    public override void Update()
    {
        switch (bossType.currentState)
        {
            case BossBase.State.Attacking:
                HandleAttacking();
                break;
            case BossBase.State.Juggling:
                HandleMoving();
                break;
            case BossBase.State.Hiding:
                HideBoss();
                break;
        }
    }

    public override void HandleAttacking()
    {
        if (!bossType.isAttacking)
            bossType.currentState = BossBase.State.Juggling;
    }

    public override void HandleMoving()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;

            if (fireCooldown <= 0f)
            {
                bossType.PickAttack();
                fireCooldown = attackCooldown;
                bossType.currentState = BossBase.State.Attacking;
            }
        }
    }

    public void HideBoss()
    {
    
    }

    public override void HandleWaiting()
    {
        throw new System.NotImplementedException();
    }
}

