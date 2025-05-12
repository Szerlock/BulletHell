using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private readonly List<Bullet> activeBullets = new();
    public static BulletManager Instance;

    private bool lastUnstableState = true;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (UIManager.Instance.backgroundUp) return;
        float dt = Time.deltaTime;

        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            Bullet b = activeBullets[i];
            b.Move(dt);

            float sqrDistance = (b.transform.position - GameManager.Instance.Player.transform.position).sqrMagnitude;
            bool shouldBeDanger = sqrDistance < (b.DangerDistance * b.DangerDistance);

            b.SetDangerState(shouldBeDanger);
        }
    }

    public void Register(Bullet bullet)
    {
        if (!activeBullets.Contains(bullet))
            activeBullets.Add(bullet);
    }

    public void Unregister(Bullet bullet)
    {
        activeBullets.Remove(bullet);
    }
}
