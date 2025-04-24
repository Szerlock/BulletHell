using NUnit.Framework;
using System;
using UnityEngine;

public abstract class AugmentBase : MonoBehaviour
{
    private AugmentSystem augmentSystem;

    public string augmentName;
    [TextArea] public string description;
    public Sprite icon;

    public virtual void Picked()
    {
        if (augmentSystem == null)
            augmentSystem = FindFirstObjectByType<AugmentSystem>();
        augmentSystem.UpdateAugmentPicker(augmentName);


        ApplyEffect();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetAugmentSystem(AugmentSystem system)
    {
        augmentSystem = system;
    }


    public abstract void ApplyEffect();
}
