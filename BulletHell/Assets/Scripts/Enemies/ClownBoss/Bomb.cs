using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionGameObject;
    float throwForce = 10f;    
    public float height = 5f;
    public float stopDistance; 
    public CircleDrawer circleDrawer;

    private Transform bombTarget;
    [SerializeField] private Rigidbody rb;
    private bool isThrown = false;
    [SerializeField] LayerMask groundMask;
    private bool BombStopped = false;

    public float distance;

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
            UpdateCirclePosition();
        }
    }

    private void MoveBombTowardsTarget()
    {
        distance = Vector3.Distance(transform.position, bombTarget.position);

        if (distance > stopDistance && !BombStopped)
        {
            //transform.position = Vector3.MoveTowards(transform.position, bombTarget.position, throwForce * Time.deltaTime);
            Vector3 direction = (bombTarget.position - transform.position);


            rb.linearVelocity = direction.normalized * throwForce;
        }
        else
        {
            StopBombMovement();
            BombStopped = true;
        }
    }

    private void StopBombMovement()
    {
        isThrown = false;
        rb.linearVelocity = Vector3.zero; 
        rb.useGravity = true;
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

    private void UpdateCirclePosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundMask))
        {
            circleDrawer.SetCirclePosition(hit.point);
        }
    }
}
