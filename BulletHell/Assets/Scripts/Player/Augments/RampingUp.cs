using UnityEngine;

public class RampingUp : AugmentBase
{
    private MinigunController minigunController;

    void Start()
    {
        minigunController = FindFirstObjectByType<MinigunController>();
    }

    public override void ApplyEffect()
    {
        minigunController.rampingUPUnlocked = true;
    }
}
