using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AugmentSystem : MonoBehaviour
{
    [SerializeField] private AugmentsPicker augmentsPicker;
    [SerializeField] private List<AugmentBase> potentialAugments;
    public List<AugmentBase> activeAugments = new List<AugmentBase>();

    public void UpdateAugmentPicker(string augmentName)
    {
        AugmentBase found = potentialAugments.FirstOrDefault(a => a.augmentName == augmentName);

        activeAugments.Add(found);

        potentialAugments.Remove(found);
        augmentsPicker.RemoveAugment(found);

    }

}
