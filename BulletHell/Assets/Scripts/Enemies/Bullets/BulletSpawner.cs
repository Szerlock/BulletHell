using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public List<BulletPatternData> patterns;
    public List<BulletPatternData> unstablePatterns;

    public Transform player;
    private int lastPatternIndex = -1;
    public BulletPatternData currentPattern;

    private float currentAngle = 0f;
    private bool attackFinished = false;
    private int bulletsFired = 0;
    private bool isFiring = false;
    private Vector3 spawnOffset = Vector3.zero;

    public BossBase boss;

    void Start()
    {
        PickNewPattern();
        player = GameManager.Instance.Player.transform;
        if (boss == null)
            boss = BossManager.Instance.currentBoss;
    }

    [ContextMenu("Fire")]
    public void StartFiring()
    {
        if (boss.isConjuring)
        return;

        if (!isFiring)
        {
            isFiring = true;
            if (boss.currentState == BossBase.State.Conjuring)
            {
                PickNewUnstablePattern();
            }
            else if (!boss.SecondPhase)
            {
                PickNewPattern();
            }
            else
            {
                PickNewUnstablePattern();
                boss.isConjuring = true;
            }
            StartCoroutine(FireBullets());
        }
    }

    public void Fire()
    { 
        StartCoroutine(FireBullets());
    }

    private IEnumerator FireBullets()
    {
        if (boss != null && boss.SecondPhase)
            yield return new WaitForSeconds(2f);
        while (bulletsFired < currentPattern.totalBullets)
        {
            SpawnWave();

            yield return new WaitForSeconds(currentPattern.fireRate);
        }

        attackFinished = true;
    }

    public void SpawnWave()
    {
        for (int l = 0; l < currentPattern.numberOfLayers; l++)
        {
            for (int i = 0; i < currentPattern.bulletsPerWave; i++)
            {
                Vector3 dir = Vector3.zero;

                switch (currentPattern.patternType)
                {
                    case PatternType.Spiral:
                        dir = SpiralPattern(i);
                        break;

                    case PatternType.Circle:
                        dir = CirclePattern(i);
                        break;

                    case PatternType.Circle4:
                        spawnOffset = Circle4Pattern(i);
                        dir = spawnOffset.normalized;
                        break;

                    case PatternType.Cone:
                        dir = Cone(i);
                        break;

                    case PatternType.DoubleHelix:
                        dir = DoubleHelixPattern(i);
                        break;

                    case PatternType.Triangle:
                        dir = TrianglePattern(i, out spawnOffset);
                        break;

                    case PatternType.Sphere:
                        dir = SpherePattern(i);
                        break;

                    case PatternType.SineWaveDisk:
                        dir = SineWarpDisk(i);
                        break;

                    case PatternType.Torus:
                        dir = TorusPattern(i);
                        break;

                    case PatternType.CubeShell:
                        dir = CubeShellPattern(i);
                        break;

                    case PatternType.Serpent:
                        dir = SerpentPattern(i);
                        break;

                    case PatternType.LayeredSpiral:
                        dir = LayeredSpiralPattern(i, l);
                        spawnOffset = Vector3.up * currentPattern.layerSpacing * l;
                        break;

                    case PatternType.Random3D:
                        dir = Random3D();
                        break;

                    case PatternType.VerticalHelix:
                        spawnOffset = VerticalMountainHelix(i);
                        dir = spawnOffset.normalized;
                        break;

                    case PatternType.Aimed:
                        if (player != null)
                        {
                            dir = (player.position - transform.position).normalized;
                        }
                        else
                        {
                            dir = Vector3.down;
                        }
                        break;

                    case PatternType.Random:
                        dir = RandomPattern();
                        break;
                }

                if (player != null)
                {
                    if (currentPattern.patternType != PatternType.Aimed)
                    {
                        Vector3 toPlayer = (player.position - transform.position).normalized;
                        Quaternion rotationToPlayer = Quaternion.LookRotation(toPlayer);
                        dir = rotationToPlayer * dir;
                    }
                }

                GameObject bullet = BulletPool.Instance.GetBullet();
                if (bullet == null)
                {
                    bulletsFired++;
                    continue;
                }
                bullet.transform.position = transform.position + spawnOffset;
                bullet.transform.rotation = Quaternion.identity;

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.InitializeBullet(dir * currentPattern.bulletSpeed, boss.Damage, currentPattern.bulletLifetime);
                bulletScript.speed = currentPattern.bulletSpeed;


                bulletsFired++;
            }

            if (currentPattern.patternType == PatternType.Spiral || currentPattern.patternType == PatternType.LayeredSpiral)
                currentAngle += currentPattern.spiralSpeed;
        }
        Debug.Log(currentPattern.name);
    }

    private Vector3 LayeredSpiralPattern(int i, int layer)
    {
        float angle = currentAngle + (360f / currentPattern.bulletsPerWave) * i;
        float rad = angle * Mathf.Deg2Rad;

        // Optional: you can make each layer rotate differently
        float layerOffset = layer * 10f; // or some variable offset
        rad += layerOffset * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
    }

    private Vector3 TorusPattern(int i)
    {
        float majorRadius = 1.5f;
        float minorRadius = 0.5f;

        float angle = (2 * Mathf.PI / currentPattern.bulletsPerWave) * i;
        float tilt = Mathf.Sin(Time.time + i * 0.1f) * Mathf.PI;

        float x = (majorRadius + minorRadius * Mathf.Cos(tilt)) * Mathf.Cos(angle);
        float y = minorRadius * Mathf.Sin(tilt);
        float z = (majorRadius + minorRadius * Mathf.Cos(tilt)) * Mathf.Sin(angle);

        return new Vector3(x, y, z).normalized;
    }

    private Vector3 CubeShellPattern(int i)
    {
        int faces = 6;
        int perFace = currentPattern.bulletsPerWave / faces;
        int faceIndex = i / perFace;
        float t = (i % perFace) / (float)perFace;

        float x = 0, y = 0, z = 0;
        float spread = 1.5f;

        switch (faceIndex)
        {
            case 0: x = -spread; y = Random.Range(-spread, spread); z = Random.Range(-spread, spread); break;
            case 1: x = spread; y = Random.Range(-spread, spread); z = Random.Range(-spread, spread); break;
            case 2: x = Random.Range(-spread, spread); y = -spread; z = Random.Range(-spread, spread); break;
            case 3: x = Random.Range(-spread, spread); y = spread; z = Random.Range(-spread, spread); break;
            case 4: x = Random.Range(-spread, spread); y = Random.Range(-spread, spread); z = -spread; break;
            case 5: x = Random.Range(-spread, spread); y = Random.Range(-spread, spread); z = spread; break;
        }

        return new Vector3(x, y, z);
    }

    private Vector3 SerpentPattern(int i)
    {
        float waveFreq = 0.5f;
        float timeOffset = Time.time * 2f;
        float angle = i * 0.3f;

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle + Mathf.Sin(i * waveFreq + timeOffset));
        float z = Mathf.Sin(angle);

        return new Vector3(x, y, z);
    }

    private Vector3 SpherePattern(int i)
    {
        int total = currentPattern.bulletsPerWave;
        float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / total);
        float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Cos(phi);
        float z = Mathf.Sin(phi) * Mathf.Sin(theta);

        return new Vector3(x, y, z);
    }

    private Vector3 Random3D()
    {
        return UnityEngine.Random.onUnitSphere.normalized;
    }

    private Vector3 SineWarpDisk(int i)
    {
        float angle = 2 * Mathf.PI * i / currentPattern.bulletsPerWave;
        float y = Mathf.Sin(Time.time * 5f + angle) * 0.5f;

        float x = Mathf.Cos(angle);
        float z = Mathf.Sin(angle);

        return new Vector3(x, y, z).normalized;
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

    private Vector3 Circle4Pattern(int i, int petals = 6)
    {
        float angle = (2 * Mathf.PI / currentPattern.bulletsPerWave) * i;
        float radius = Mathf.Sin(petals * angle);

        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);

        return new Vector3(x, 0f, z).normalized;
    }

    private Vector3 Cone(int i)
    {
        float angle = currentAngle + i * 10f;
        float rad = angle * Mathf.Deg2Rad;
        float radius = 0.1f * i;
        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
    }

    private Vector3 TrianglePattern(int i, out Vector3 spawnPosition)
    {
        int bullets = currentPattern.bulletsPerWave;
        int sides = 3;
        int bulletsPerSide = bullets / sides;

        float size = 3f;

        Vector3 p0 = new Vector3(0, 0, size);                     
        Vector3 p1 = new Vector3(-size * 0.866f, 0, -size * 0.5f); 
        Vector3 p2 = new Vector3(size * 0.866f, 0, -size * 0.5f); 

        int sideIndex = i / bulletsPerSide;
        float t = (i % bulletsPerSide) / (float)bulletsPerSide;

        Vector3 a = Vector3.zero, b = Vector3.zero;

        switch (sideIndex)
        {
            case 0: a = p0; b = p1; break;
            case 1: a = p1; b = p2; break;
            case 2: a = p2; b = p0; break;
        }

        // Position on edge of triangle
        spawnPosition = Vector3.Lerp(a, b, t);

        // Direction = outward from triangle center
        return (spawnPosition - Vector3.zero).normalized;
    }

    private Vector3 DoubleHelixPattern(int i)
    {
        float angle = currentAngle + (360f / currentPattern.bulletsPerWave) * i;
        float rad = angle * Mathf.Deg2Rad;

        float flip = (i % 2 == 0) ? 1f : -1f;

        float offset = Mathf.Sin(Time.time * currentPattern.spiralSpeed + i) * 0.5f;

        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) + new Vector3(offset * flip, 0f, 0f);
    }
    private Vector3 VerticalMountainHelix(int i)
    {
        float angle = (360f / currentPattern.bulletsPerWave) * i;
        float rad = angle * Mathf.Deg2Rad;

        float radius = 5f;
        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;

        float frequency = 1.5f;
        float speed = 2f;
        float amplitude = 2f;

        float y = Mathf.Sin(x * frequency + Time.time * speed) * amplitude;

        return new Vector3(x, y, z);
    }

    public bool AttackFinished()
    {
        return attackFinished;
    }

    public void PickNewPattern()
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

    public void SetPattern(BulletPatternData pattern)
    { 
        currentPattern = pattern;
    }

    public void PickSpecificPattern(string name)
    {
        if (Enum.TryParse(name, out PatternType parsedPattern))
        {
            BulletPatternData pattern = patterns.Find(p => p.patternType == parsedPattern);
        }
        StartCoroutine(FireBullets());
    }

    private void PickNewUnstablePattern()
    {
        int index;
        do
        {
            index = Random.Range(0, unstablePatterns.Count);
        }
        while (index == lastPatternIndex && unstablePatterns.Count > 1);

        currentPattern = unstablePatterns[index];
        lastPatternIndex = index;
    }

    public void ResetAttack()
    {
        currentAngle = 0f;
        bulletsFired = 0;
        isFiring = false;
    }
}
