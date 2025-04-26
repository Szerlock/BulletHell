using UnityEngine;

public class IncreaseMaxHealth : AugmentBase
{
    public float healthIncreaseAmount;

    public override void ApplyEffect()
    {
        GameManager.Instance.Player.SetMaxHealth(healthIncreaseAmount);
    }

    public override void Picked()
    {
        base.Picked();
    }
}
