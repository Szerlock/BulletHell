using System.Collections.Generic;
using UnityEngine;
using UnityEngine.DedicatedServer;
using UnityEngine.UI;

public class AugmentsPicker : MonoBehaviour
{
    [SerializeField] private List<AugmentBase> potentialAugments;
    [SerializeField] private List<GameObject> augmentUITabs;
    [SerializeField] private List<Transform> augmentUIPos;
    [SerializeField] private Transform augmentPanel;
    [SerializeField] private List<GameObject> UITabsShowing;

    [ContextMenu("PickAugment")]
    public void PickAugment()
    {

        if (potentialAugments.Count < 3)
        {
            Debug.Log("Less Than 3 augments");
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

    public void ShowAugment(GameObject AugmentTab, int i, AugmentBase augment)
    {
        GameObject tab = Instantiate(AugmentTab, augmentUIPos[i].position, Quaternion.identity, augmentPanel);
        UITabsShowing.Add(tab);
        tab.GetComponent<AugmentUITab>().Init(augment);
    }

    public void RemoveAugment(AugmentBase augment)
    {
        if (potentialAugments.Contains(augment))
        {
            potentialAugments.Remove(augment);
            augmentUITabs.Remove(augment.gameObject);
            CloseUI();
        }
    }

    private void CloseUI()
    {
        foreach (GameObject tab in UITabsShowing)
        {
            Destroy(tab);
        }
    }
}

