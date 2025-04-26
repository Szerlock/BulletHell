using UnityEngine;

public class IncreasePower : AugmentBase
{
    public float multiplier;

    public override void ApplyEffect()
    {
        GameManager.Instance.Player.IncreasePower(multiplier);
    }

    public override void Picked()
    {
        base.Picked();
    }
}
