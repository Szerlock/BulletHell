using UnityEngine;

public class HealAllHp : AugmentBase
{
    public override void ApplyEffect()
    {
        GameManager.Instance.Player.HealToFull();
    }

    public override void Picked()
    {
        base.Picked();
    }
}
