using UnityEngine;

public class SpiralSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int bulletsPerWave = 10;
    public float timeBetweenWaves = 0.1f;
    public float spiralSpeed = 10f;

    private float currentAngle = 0f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnWave), 0f, timeBetweenWaves);
    }

    void SpawnWave()
    {
        for (int i = 0; i < bulletsPerWave; i++)
        {
            float angle = currentAngle + (360f / bulletsPerWave) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            GameObject bullet = GetBullet();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetActive(true);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDirection(dir);
            bulletScript.DestroyAfter(5f);
        }

        currentAngle += spiralSpeed;
    }

    GameObject GetBullet()
    {
        // Replace this with a pool manager in production
        return Instantiate(bulletPrefab);
    }
}
