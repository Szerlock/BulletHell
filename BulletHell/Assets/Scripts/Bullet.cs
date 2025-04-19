using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void DestroyAfter(float seconds)
    {
        Invoke(nameof(Disable), seconds);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}
