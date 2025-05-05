using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] private int segments = 100;  
    [SerializeField] private float radius = 5f; 
    private LineRenderer lineRenderer;
    [SerializeField] private Material lineMaterial;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;

        lineRenderer.useWorldSpace = true;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        DrawCircle();
    }

    void Update()
    {
        if (lineRenderer != null)
        {
            DrawCircle();
        }
    }

    private void DrawCircle()
    {
        float angle = 0f;
        float angleStep = 360f / segments;

        Vector3 center = transform.position;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, 0, z) + center);

            angle += angleStep;
        }
    }

    public void SetCirclePosition(Vector3 position)
    {
        transform.position = position;
    }
}
