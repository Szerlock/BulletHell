using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    [SerializeField] private float smoothSpeed = 10f;
    private float targetHealth;
    public static HealthBar Instance;

    private void Start()
    {
        Instance = this;
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        targetHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        targetHealth = health;
    }

    void Update()
    {
        if (healthSlider.value != targetHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, Time.deltaTime * smoothSpeed);
        }
    }
}
