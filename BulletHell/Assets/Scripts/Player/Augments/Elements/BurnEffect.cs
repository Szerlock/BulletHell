using UnityEngine;

[System.Serializable]
public class BurnEffect
{
    public float tickDamage;
    public float tickInterval;
    public float duration;

    public BurnEffect(float tickDamage, float tickInterval, float duration)
    {
        this.tickDamage = tickDamage;
        this.tickInterval = tickInterval;
        this.duration = duration;
    }
}
