using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentsPicker : MonoBehaviour
{
    [SerializeField] private List<AugmentBase> potentialAugments;
    [SerializeField] private List<GameObject> augmentUITabs;
    [SerializeField] private List<Transform> augmentUIPos;
    [SerializeField] private Transform augmentPanel;
    [SerializeField] private List<GameObject> UITabsShowing;

    public bool finishedPickingAugment = false;
    private bool augmentPicked = false;
    private int augmentPickedCount = 0;
    private int AugmentNeeded = 0;

    public IEnumerator StartAugmentPicking(int amount)
    {
        augmentPickedCount = 0;
        AugmentNeeded = amount;
        while (augmentPickedCount != AugmentNeeded)
        {
            PickAugment();
            yield return new WaitUntil(() => augmentPicked == true);
        }
    }


    [ContextMenu("PickAugment")]
    public void PickAugment()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        augmentPicked = false;

        List<int> usedIndices = new List<int>();
        int count = Mathf.Min(3, potentialAugments.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, potentialAugments.Count);
            }
            while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);

            AugmentBase pickedAugment = potentialAugments[randomIndex];
            GameObject tab = augmentUITabs[randomIndex];

            ShowAugment(tab, i, pickedAugment);
            //Debug.Log($"Picked augment {pickedAugment.name} for tab index {i}");

        }
    }

    public void ShowAugment(GameObject AugmentTab, int i, AugmentBase augment)
    {
        GameObject tab = Instantiate(AugmentTab, augmentUIPos[i].position, Quaternion.identity, augmentPanel);
        UITabsShowing.Add(tab);
        tab.GetComponent<AugmentUITab>().Init(augment);
        //Debug.Log($"Initializing tab with augment: {augment.name}");

    }

    public void RemoveAugment(AugmentBase augment)
    {
        int index = potentialAugments.FindIndex(aug => aug == augment);
        if (index != -1)
        {
            augmentUITabs.RemoveAt(index);
            potentialAugments.Remove(augment);
            augmentPicked = true;
            augmentPickedCount++;
            RemoveCurrentAugmentTabs();

            if (augmentPickedCount == AugmentNeeded)
            CloseUI();
        }
    }

    private void RemoveCurrentAugmentTabs()
    {
        foreach (GameObject tab in UITabsShowing)
        {
            Destroy(tab);
        }
        UITabsShowing.Clear();
    }

    private void CloseUI()
    {
        finishedPickingAugment = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        RemoveCurrentAugmentTabs();

        // Remove Image
    }
}

