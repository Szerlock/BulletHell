using UnityEngine;

public class Invincibility : AugmentBase
{
    public override void ApplyEffect()
    {
        GameManager.Instance.Player.isInvincibleUnlocked = true;
    }

    public override void Picked()
    {
        base.Picked();
    }
}
