using UnityEngine;

public class BallerinaUnit : MonoBehaviour
{
    private MiniBallerinaBoss boss;
    private Vector3 targetPosition;
    [SerializeField] private float rotateSpeed = 360f;

    public void Init(MiniBallerinaBoss bossController)
    {
        boss = bossController;
    }

    public void MoveToPosition(Vector3 position)    
    {
        targetPosition = position + Vector3.up * 7.5f;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
