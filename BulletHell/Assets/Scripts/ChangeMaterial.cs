using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private MeshRenderer meshRenderer; 
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material unstableMaterial;


    public void ChangeMat(bool isUnstable)
    {
        if (skinnedMeshRenderer != null)
        {
            List<Material> materialList = new List<Material>(skinnedMeshRenderer.materials);

            for (int i = 0; i < materialList.Count; i++)
            {
                if (isUnstable)
                {
                    materialList[i] = unstableMaterial;
                }
                else if (!isUnstable)
                {
                    materialList[i] = originalMaterial;
                }
            }

            skinnedMeshRenderer.materials = materialList.ToArray();
        }

        if (meshRenderer != null)
        {
            meshRenderer.material = isUnstable ? unstableMaterial : originalMaterial;
        }
    }
}
