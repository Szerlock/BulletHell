using UnityEngine;

public class BulletsBurn : AugmentBase
{
    private MinigunController minigunController;

    public override void Picked()
    {
        base.Picked();
    }
    public override void ApplyEffect()
    {
        minigunController = FindFirstObjectByType<MinigunController>();
        minigunController.bulletsBurnUnlocked = true;
    }
}
