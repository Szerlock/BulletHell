using UnityEngine;

public class OilUrnProyectile : MonoBehaviour
{
    public GameObject oilZonePrefab;
    public float speed = 20f;
    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Instantiate(oilZonePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
