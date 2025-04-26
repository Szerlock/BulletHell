using UnityEngine;
using static DragonType;

public class Dragon : AugmentBase
{
    [SerializeField] private DragonAugmentType dragonType;
    private CharacterController3D playerController;
    public override void ApplyEffect()
    {
        playerController = FindFirstObjectByType<CharacterController3D>();
        switch (dragonType)
        {
            case DragonAugmentType.BombDragon:
                playerController.SpawnBombDragon();
                break;
            case DragonAugmentType.FireDragon:
                playerController.SpawnFireDragon();
                break;
            case DragonAugmentType.HealingDragon:
                playerController.SpawnHealingDragon();
                break;
            case DragonAugmentType.ShadowDragon:
                playerController.SpawnShadowDragon();
                break;
        }
    }

    public override void Picked()
    {
        base.Picked();
    }
}
