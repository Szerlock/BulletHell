using System.Collections;
using UnityEngine;

public class FlashingEffect : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] originalMaterials;
    public Color flashColor = Color.white; 
    [SerializeField] private float flashDuration;
    [SerializeField] private float flashCount;
    private float iframeDuration;

    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        originalMaterials = new Material[skinnedMeshRenderer.materials.Length];
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            originalMaterials[i] = new Material(skinnedMeshRenderer.materials[i]);
        }
    }

    public void Flash(float iframeDur)
    {
        StartCoroutine(FlashCoroutine());
        flashDuration = iframeDuration / (flashCount * 2);
        iframeDuration = iframeDur;
    }

    private IEnumerator FlashCoroutine()
    {
        float timer = 0f;

        while (timer < iframeDuration)
        {
            // Flash color
            SetAllMaterialsColor(flashColor);
            yield return new WaitForSeconds(flashDuration);

            // Original color
            ResetAllMaterialsColor();
            yield return new WaitForSeconds(flashDuration);

            timer += flashDuration * 2;
        }
    }

    void SetAllMaterialsColor(Color color)
    {
        foreach (var mat in skinnedMeshRenderer.materials)
            mat.color = color;
    }

    void ResetAllMaterialsColor()
    {
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            skinnedMeshRenderer.materials[i].color = originalMaterials[i].color;
    }
}
