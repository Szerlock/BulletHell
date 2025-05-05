using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public float damage;

    private void Start()
    {
        // play explosion vfx
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController3D playerHealth = other.GetComponent<CharacterController3D>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
