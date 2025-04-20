using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletPattern", menuName = "Bullet Hell/Bullet Pattern")]
public class BulletPatternData : ScriptableObject
{
    public PatternType patternType;
    public int bulletsPerWave;
    public float timeBetweenWaves;
    public float spiralSpeed;
    public float bulletSpeed;
    public float bulletLifetime;

    public float fireRate;
    public int totalBullets;
}

public enum PatternType
{
    Spiral,
    Circle,
    Aimed,
    Random,
}