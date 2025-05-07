using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize;
    [SerializeField] private int maxPoolSize;
    [SerializeField] private int expand;

    public Queue<GameObject> pool = new Queue<GameObject>();
    private int currentSize;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
        currentSize = initialPoolSize;
    }

    private IEnumerator ExpandPoolCoroutine()
    {
        int expandCount = Mathf.Min(expand, maxPoolSize - currentSize);
        int batchSize = 10;

        for (int i = 0; i < expandCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
            currentSize++;

            if (i % batchSize == 0)
                yield return null;
        }
    }

    public GameObject GetBullet()
    {
        if (pool.Count == 0)
        {
            ExpandPoolAsync(this);
            Debug.LogWarning("Bullet pool empty — expanding. Returning null.");
            return null;
        }

        GameObject bullet = pool.Dequeue();
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null) return;

        bullet.SetActive(false);

        pool.Enqueue(bullet);
    }

    public void ExpandPoolAsync(MonoBehaviour runner)
    {
        runner.StartCoroutine(ExpandPoolCoroutine());
    }
}
