using UnityEngine;

public class CritChanceAugment : AugmentBase
{
    CharacterController3D playerController;

    public override void Picked()
    { 
        base.Picked();
    }

    public override void ApplyEffect()
    {
        playerController = FindFirstObjectByType<CharacterController3D>();
        playerController.critChance += 0.2f;
    }
}
