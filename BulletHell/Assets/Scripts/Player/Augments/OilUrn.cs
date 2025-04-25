using UnityEngine;

public class OilUrn : AugmentBase
{
    private CharacterController3D player;

    public override void Picked()
    {
        base.Picked();
    }
    public override void ApplyEffect()
    {
        player = FindFirstObjectByType<CharacterController3D>();
        player.OilUrnUnlocked = true;
    }

}
