using UnityEngine;

public class DamageDistance : AugmentBase
{
    public override void Picked()
    {
        base.Picked();
    }

    public override void ApplyEffect()
    {
        MinigunController minigunController = FindFirstObjectByType<MinigunController>();
        minigunController.distanceUnlocked = true;
    }
}
