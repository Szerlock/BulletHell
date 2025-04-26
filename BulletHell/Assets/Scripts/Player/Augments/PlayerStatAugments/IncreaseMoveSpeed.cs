using UnityEngine;

public class IncreaseMoveSpeed : AugmentBase
{
    public float multiplier;

    public override void ApplyEffect()
    {
        GameManager.Instance.Player.IncreaseMoveSpeed(multiplier);
    }

    public override void Picked()
    {
        base.Picked();
    }
}
