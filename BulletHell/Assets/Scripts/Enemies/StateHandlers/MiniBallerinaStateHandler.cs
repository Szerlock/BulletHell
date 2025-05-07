using UnityEngine;

public class MiniBallerinaStateHandler : StateHandler
{
    private MiniBallerinaBoss ballerina;

    public override void HandleAttacking()
    {
        throw new System.NotImplementedException();
    }

    public override void HandleMoving()
    {
        throw new System.NotImplementedException();
    }

    public override void HandleWaiting()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(BossBase bossInstance)
    {
        ballerina = bossInstance as MiniBallerinaBoss;
    }

    public override void Update()
    {
    }
}
