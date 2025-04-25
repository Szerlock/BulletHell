using System.Collections;
using UnityEngine;

public class FlashingEffect : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] originalMaterials;
    public Color flashColor = Color.white; 
    public float flashDuration;   

    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        originalMaterials = new Material[skinnedMeshRenderer.materials.Length];
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            originalMaterials[i] = new Material(skinnedMeshRenderer.materials[i]);
        }
    }

    public void Flash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        int flashCount = 2;

        for (int j = 0; j < flashCount; j++)
        {
            // Set flash color
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration);

            // Reset to original color
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].color = originalMaterials[i].color;
            }

            yield return new WaitForSeconds(flashDuration);
        }
    }
}
