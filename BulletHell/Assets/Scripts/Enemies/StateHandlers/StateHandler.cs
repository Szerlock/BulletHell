using UnityEngine;

public abstract class StateHandler : MonoBehaviour
{
    protected BossBase boss;
    protected float fireCooldown;

    public abstract void Init(BossBase bossInstance);

    public abstract void Update();

    public abstract void HandleWaiting();
    public abstract void HandleAttacking();
    public abstract void HandleMoving();
}