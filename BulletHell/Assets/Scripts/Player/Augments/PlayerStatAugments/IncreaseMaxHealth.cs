using UnityEngine;

public class IncreaseMaxHealth : AugmentBase
{
    public float healthIncreaseAmount;

    public override void ApplyEffect()
    {
        GameManager.Instance.Player.SetMaxHealth();
    }

    public override void Picked()
    {
        base.Picked();
    }
}
