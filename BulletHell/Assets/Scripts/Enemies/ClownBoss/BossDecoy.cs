using UnityEngine;

public class BossDecoy : EnemyBase
{
    public BulletSpawner bulletSpawner;
    public Transform target;
    public float interval;

    public void Init()
    {
        Debug.Log("Init called. bulletSpawner = " + bulletSpawner);
        bulletSpawner.player = target;
        InvokeRepeating(nameof(CallPickSpecificPattern), 0f, interval);
    }

    private void CallPickSpecificPattern()
    {
        bulletSpawner.ResetAttack();
        bulletSpawner.PickSpecificPattern("Cone");
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.currentBoss.GetComponent<ClownBoss>().bossDecoys.Remove(gameObject);
    }
}
