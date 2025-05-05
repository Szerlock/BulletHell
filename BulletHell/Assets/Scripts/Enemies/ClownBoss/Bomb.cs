using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject bombPrefab;    
    public float throwForce = 10f;    
    public float height = 5f;
    public float stopDistance = 0.5f; 
    public LineRenderer circleRenderer;

    private Transform bombTarget;
    [SerializeField] private Rigidbody rb;
    private bool isThrown = false;

    public void Init(Transform target)
    {
        bombTarget = target;
    }

    private void Update()
    {
        MoveBombTowardsTarget();
    }

    private void MoveBombTowardsTarget()
    {
        // Calculate the distance between the bomb and the target
        float distance = Vector3.Distance(transform.position, bombTarget.position);

        // If the bomb is not close enough to the target, move it
        if (distance > stopDistance)
        {
            // Move bomb position smoothly towards the target
            transform.position = Vector3.MoveTowards(transform.position, bombTarget.position, throwForce * Time.deltaTime);
        }
        else
        {
            // Stop the bomb's movement once it's close enough
            StopBombMovement();
        }
    }

    // Stop the bomb's movement and apply gravity
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
        if (bombTarget == null) return;

        // Calculate direction towards the target
        Vector3 direction = bombTarget.position - transform.position;

        // Add height (Y component) for an arc trajectory
        direction.y = height;

        // Apply the throw force
        rb.AddForce(direction.normalized * throwForce, ForceMode.Impulse);
    }
}
