using System.Collections.Generic;
using UnityEngine;
using UnityEngine.DedicatedServer;
using UnityEngine.UI;

public class AugmentsPicker : MonoBehaviour
{
    [SerializeField] private List<AugmentBase> potentialAugments;
    [SerializeField] private List<GameObject> augmentUITabs;
    [SerializeField] private List<Transform> augmentUIPos;

    public void PickAugment()
    {
        List<int> usedIndices = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, potentialAugments.Count);
            }
            while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);

            AugmentBase pickedAugment = potentialAugments[randomIndex];
            GameObject tab = augmentUITabs[i];

            ShowAugment(tab, i, pickedAugment);
        }
    }

    public void ShowAugment(GameObject PickedAugment, int i, AugmentBase augment)
    {
        PickedAugment.SetActive(true);
        PickedAugment.transform.position = augmentUIPos[i].position;
        PickedAugment.GetComponent<Button>().onClick.AddListener(() =>
        {
            augment.Picked();
        });
    }
    public void RemoveAugment(AugmentBase augment)
    {
        if (potentialAugments.Contains(augment))
        {
            potentialAugments.Remove(augment);
        }
    }
}
