using UnityEngine;

public class TrackingBullets : AugmentBase
{
    public override void ApplyEffect()
    {
        MinigunController minigunController = FindFirstObjectByType<MinigunController>();
        minigunController.trackingUnlocked = true;
    }

    public override void Picked()
    {
        base.Picked();
    }


}
