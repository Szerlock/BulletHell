using UnityEngine;

public class BurnsMoreEffective : AugmentBase
{
    public override void Picked()
    {
        base.Picked();
    }

    [ContextMenu("Test ApplyEffect")]
    public override void ApplyEffect()
    {
        BurnEffect burnEffect = GlobalBurnStats.GetBurnEffect();
        GlobalBurnStats.SetBurnStats(burnEffect.tickDamage*1.5f, burnEffect.tickInterval, burnEffect.duration * 1.5f);
    }
}
