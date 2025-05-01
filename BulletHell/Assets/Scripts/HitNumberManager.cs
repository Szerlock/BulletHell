using UnityEngine;

public class HitNumberManager : MonoBehaviour
{
    public static HitNumberManager Instance { get; private set; }

    [Header("Floating Text Variable")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Canvas textCanvas;

    private void Start()
    {
        Instance = this;

    }

    public void ShowHitNumber(Vector3 position, float damage, Transform unit)
    {
        GameObject hitText = Instantiate(floatingTextPrefab, position, Quaternion.identity);

        var textScript = hitText.GetComponent<FloatingText>();
        textScript.unit = unit;
        textScript.SetText(damage.ToString());
    }
}
