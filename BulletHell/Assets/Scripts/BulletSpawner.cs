using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public List<BulletPatternData> patterns;
    public Transform player;
    private int lastPatternIndex = -1;
    private BulletPatternData currentPattern;

    private float currentAngle = 0f;
    private bool attackFinished = false;
    private int bulletsFired = 0;
    private bool isFiring = false;

    void Start()
    {
        PickNewPattern();
    }

    public void StartFiring()
    {
        if (!isFiring)
        {
            isFiring = true;
            PickNewPattern();
            StartCoroutine(FireBullets());
        }
    }

    private IEnumerator FireBullets()
    {
        while (bulletsFired < currentPattern.totalBullets)
        {
            SpawnWave();

            yield return new WaitForSeconds(currentPattern.fireRate);
        }

        attackFinished = true;
    }

    public void SpawnWave()
    {
        for (int i = 0; i < currentPattern.bulletsPerWave; i++)
        {
            Vector3 dir = Vector3.zero;

            switch (currentPattern.patternType)
            {
                case PatternType.Spiral:
                    dir = SpiralPattern(i);
                    Debug.Log("Spiral Pattern");
                    break;

                case PatternType.Circle:
                    dir = CirclePattern(i);
                    Debug.Log("Circle Pattern");
                    break;

                //case PatternType.Aimed:
                //    if (player != null)
                //    {
                //        dir = (player.position - transform.position).normalized;
                //    }
                //    else
                //    {
                //        dir = Vector3.down;
                //    }
                //    break;

                case PatternType.Random:
                    dir = RandomPattern();
                    Debug.Log("Random Pattern");
                    break;
            }

            GameObject bullet = GetBullet();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetActive(true);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDirection(dir * currentPattern.bulletSpeed);
            bulletScript.DestroyAfter(currentPattern.bulletLifetime);

            bulletsFired++;
        }

        if (currentPattern.patternType == PatternType.Spiral)
            currentAngle += currentPattern.spiralSpeed;
    }

    private Vector3 CirclePattern(int i)
    {
        Vector3 dir;
        float circleAngle = (360f / currentPattern.bulletsPerWave) * i;
        float circleRad = circleAngle * Mathf.Deg2Rad;
        dir = new Vector3(Mathf.Cos(circleRad), 0f, Mathf.Sin(circleRad));
        return dir;
    }

    private static Vector3 RandomPattern()
    {
        Vector3 dir;
        float randAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        dir = new Vector3(Mathf.Cos(randAngle), 0f, Mathf.Sin(randAngle));
        return dir;
    }

    private Vector3 SpiralPattern(int i)
    {
        Vector3 dir;
        float angle = currentAngle + (360f / currentPattern.bulletsPerWave) * i;
        float rad = angle * Mathf.Deg2Rad;
        dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
        return dir;
    }

    public bool AttackFinished()
    {
        if (attackFinished)
        {
            attackFinished = false;
            return true;
        }
        return false;
    }

    private void PickNewPattern()
    {
        int index;
        do
        {
            index = Random.Range(0, patterns.Count);
        }
        while (index == lastPatternIndex && patterns.Count > 1);

        currentPattern = patterns[index];
        lastPatternIndex = index;
    }

    private GameObject GetBullet()
    {
        // Replace this with a pool manager in production
        return Instantiate(bulletPrefab);
    }

    public void ResetAttack()
    {
        currentAngle = 0f;
        bulletsFired = 0;
        isFiring = false;
    }
}
