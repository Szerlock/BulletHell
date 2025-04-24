using UnityEngine;

public class BulletsBurn : AugmentBase
{
    private MinigunController minigunController;

    void Start()
    {
        minigunController = FindFirstObjectByType<MinigunController>();
    }

    public override void ApplyEffect()
    {
        minigunController.bulletsBurnUnlocked = true;
    }
}
