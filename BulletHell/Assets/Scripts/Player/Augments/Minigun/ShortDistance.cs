using UnityEngine;

public class ShortDistance : AugmentBase
{
    public override void Picked()
    {
        base.Picked();
    }

    public override void ApplyEffect()
    {
        MinigunController minigunController = FindFirstObjectByType<MinigunController>();
        minigunController.shortDistanceUnlocked = true;
    }
}
