using UnityEngine;

public class BurnsMoreEffective : AugmentBase
{
    public override void Picked()
    {
        base.Picked();
        ApplyEffect();
    }

    [ContextMenu("Test ApplyEffect")]
    public override void ApplyEffect()
    {
        BurnEffect burnEffect = GlobalBurnStats.GetBurnEffect();
        GlobalBurnStats.SetBurnStats(burnEffect.tickDamage*2, burnEffect.tickInterval, burnEffect.duration * 2);
    }
}
