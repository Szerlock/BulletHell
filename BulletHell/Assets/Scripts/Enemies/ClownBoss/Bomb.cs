using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject explosionGameObject;
    float throwForce = 10f;    
    public float height = 5f;
    public float stopDistance = 0.5f; 
    public CircleDrawer circleRenderer;

    private Transform bombTarget;
    [SerializeField] private Rigidbody rb;
    private bool isThrown = false;

    public void Init(Transform target)
    {
        bombTarget = target;
        ThrowBomb();
    }

    private void Update()
    {
        if (isThrown)
        {   
            MoveBombTowardsTarget();
        }
    }

    private void MoveBombTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, bombTarget.position);

        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, bombTarget.position, throwForce * Time.deltaTime);
        }
        else
        {
            StopBombMovement();
        }
    }

    private void StopBombMovement()
    {
        isThrown = false;

        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void ThrowBomb()
    {
        isThrown = true;
        if (bombTarget == null) return;

        Vector3 direction = bombTarget.position - transform.position;

        direction.y = height;

        rb.AddForce(direction.normalized * throwForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
            Instantiate(explosionGameObject, transform.position, Quaternion.identity);
    }
}
