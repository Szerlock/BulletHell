using NUnit.Framework;
using System;
using UnityEngine;

public abstract class AugmentBase : MonoBehaviour
{
    private AugmentSystem augmentSystem;
    public string augmentName;
    [TextArea] public string description;
    public Sprite icon;

    private void Start()
    {
        augmentSystem = FindFirstObjectByType<AugmentSystem>();
    }

    public virtual void Picked()
    {
        augmentSystem.UpdateAugmentPicker(augmentName);
        ApplyEffect();
    }

    public abstract void ApplyEffect();
}
