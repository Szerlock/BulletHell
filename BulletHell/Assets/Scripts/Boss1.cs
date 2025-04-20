using UnityEngine;

public class Boss1 : EnemyBase
{
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private float fireRate;
    [SerializeField] private float fireCooldown;

    private enum State
    {
        Waiting,
        Attacking,
        AttackAgain,
        Death
    }

    private State currentState = State.Waiting;

    void Update()
    {
        switch (currentState)
        {
            case State.Waiting:
                HandleWaitingState();
                break;
            case State.Attacking:
                HandleAttackingState();
                break;
        }
    }

    void HandleWaitingState()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            currentState = State.Attacking;
        }
    }

    void HandleAttackingState()
    {
        fireCooldown = fireRate;
        bulletSpawner.StartFiring();


        if (bulletSpawner.AttackFinished())
        {
            currentState = State.Waiting;
            bulletSpawner.ResetAttack();
        }
    }

    public override void StartPhase()
    {
        currentState = State.Waiting;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        base.Die();
        currentState = State.Death;
        Debug.Log($"{gameObject.name} State{currentState.ToString()}");
    }
}
