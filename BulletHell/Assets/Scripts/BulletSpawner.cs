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
    private Vector3 bulletPositionOffset = Vector3.zero;
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
                case PatternType.Flower:
                    dir = FlowerPattern(i);
                    Debug.Log("Flower Pattern");
                    break;
                case PatternType.Cone:
                    dir = ExpandingSpiral(i);
                    Debug.Log("Expanding Spiral Pattern");
                    break;
                case PatternType.DoubleHelix:
                    dir = DoubleHelixPattern(i);
                    Debug.Log("Double Helix Pattern");
                    break;
                case PatternType.Grid:
                    List<Vector3> gridPositions = GridPattern(5, 1f, transform.position);
                    Vector3 bulletPositionOffset = gridPositions[i % gridPositions.Count];
                    dir = Vector3.forward;
                    Debug.Log("Grid Pattern");
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
            bullet.transform.position = transform.position + bulletPositionOffset;
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

    private Vector3 FlowerPattern(int i, int arms = 5)
    {
        float angle = (360f / currentPattern.bulletsPerWave) * i;
        float armOffset = Mathf.Sin(Time.time * 5f + i) * 20f; // adds petal wobble
        float finalAngle = angle + (360f / arms) * (i % arms) + armOffset;
        float rad = finalAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
    }

    private Vector3 ExpandingSpiral(int i)
    {
        float angle = currentAngle + i * 10f; // tighter spiral
        float rad = angle * Mathf.Deg2Rad;
        float radius = 0.1f * i; // expands out
        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
    }

    private Vector3 DoubleHelixPattern(int i)
    {
        float angle = currentAngle + (360f / currentPattern.bulletsPerWave) * i;
        float rad = angle * Mathf.Deg2Rad;

        float flip = (i % 2 == 0) ? 1f : -1f;

        float offset = Mathf.Sin(Time.time * currentPattern.spiralSpeed + i) * 0.5f;

        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) + new Vector3(offset * flip, 0f, 0f);
    }

    private List<Vector3> GridPattern(int cols, float spacing, Vector3 origin)
    {
        List<Vector3> positions = new List<Vector3>();

        for (int x = 0; x < cols; x++)
        {
            float offsetX = (x - cols / 2f) * spacing;

            // Apply your specific tweak logic
            if (x == 0) offsetX += 0.25f;
            if (x == 1) offsetX += 0.25f;
            if (x == 2) offsetX -= 0.25f;
            if (x == 3) offsetX -= 0.25f;

            Vector3 newPos = origin + new Vector3(offsetX, 0f, 0f);
            positions.Add(newPos);
        }

        return positions;
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
