using UnityEngine;

public class RampingUp : AugmentBase
{
    private MinigunController minigunController;

    public override void Picked()
    {
        base.Picked();
        ApplyEffect();
    }

    public override void ApplyEffect()
    {
        minigunController = FindFirstObjectByType<MinigunController>();
        minigunController.rampingUPUnlocked = true;
    }
}
