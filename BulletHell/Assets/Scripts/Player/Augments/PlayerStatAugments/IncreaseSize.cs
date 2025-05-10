using UnityEngine;

public class IncreaseSize : AugmentBase
{
    public Transform scale;
    public float healthMultiplier;
    public float powerMultiplier;

    public override void ApplyEffect()
    {
        CharacterController3D player = GameManager.Instance.Player;
        player.IncreaseSize(scale.localScale);
        player.IncreasePower(powerMultiplier);
        player.SetMaxHealth(1);
    }

    public override void Picked()
    {
        base.Picked();
    }
}
