using System.Data.Common;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    private float damage;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void DestroyAfter(float seconds)
    {
        Invoke(nameof(Disable), seconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CharacterController3D player = other.GetComponent<CharacterController3D>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else
        {
            Disable();
        }
    }

    void Disable()
    {
        // Send back to pool
        gameObject.SetActive(false);
    }
}
