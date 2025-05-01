using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{

    private Transform mainCam;
    public Transform unit;
    private Transform textCanvas;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float lifetime = 1f;
    public Vector3 initialOffset;



    void Start()
    {
        mainCam = Camera.main.transform;
        initialOffset = transform.localPosition;

        textCanvas = GameObject.Find("TextCanvas").transform;
        transform.SetParent(textCanvas);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        initialOffset += Vector3.up * floatSpeed * Time.deltaTime;

        transform.position = unit.position + initialOffset;
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }
        
    public void SetText(string value)
    {
        textMesh.text = value;
    }
}
