using UnityEngine;

public class MiniBallerinaStateHandler : StateHandler
{
    private MiniBallerinaBoss ballerina;
    private float currentfireCooldown;

    public override void Init(BossBase bossInstance)
    {
        ballerina = bossInstance as MiniBallerinaBoss;
        currentfireCooldown = ballerina.fireCooldown;
    }

    public override void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            if (ballerina)
                if (ballerina.isInitialized)
                {
                    switch (ballerina.currentState)
                    {
                        case BossBase.State.Attacking:
                            HandleAttacking();
                            break;
                        case BossBase.State.Waiting:
                            HandleWaiting();
                            break;
                        case BossBase.State.Unstable:
                            HandleUnstable();
                            break;
                    }
                }
        }
    }

    public override void HandleAttacking()
    {
        currentfireCooldown = ballerina.fireCooldown;

        if(!ballerina.isFiring)
        ballerina.BallerinasFire();

        if (ballerina.IsBallerinaAttackFinished())
        {
            ballerina.currentState = BossBase.State.Waiting;
            ballerina.ResetAttack();
        }
    }

    public override void HandleWaiting()
    {
        currentfireCooldown -= Time.deltaTime;

        if(currentfireCooldown <= 0f)
            ballerina.currentState = BossBase.State.Unstable;

        if (currentfireCooldown <= 0f)
        {
            ballerina.currentState = BossBase.State.Attacking;
        }
    }

    public void HandleUnstable()
    {
        currentfireCooldown = ballerina.fireCooldown;

        if (!ballerina.isFiring)
            ballerina.UnstableAttack();

        if (ballerina.IsBallerinaAttackFinished())
        {
            ballerina.currentState = BossBase.State.Waiting;
            ballerina.ResetAttack();
        }
    }

    public override void HandleMoving()
    {
        throw new System.NotImplementedException();
    }
}
