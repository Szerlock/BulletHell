using System.Collections;
using UnityEngine;

public class GlobalBurnStats : MonoBehaviour
{
    public static float tickDamage = 10f;
    public static float tickInterval = 0.5f;
    public static float duration = 4f;

    public static BurnEffect GetBurnEffect()
    {
        return new BurnEffect(tickDamage, tickInterval, duration);
    }

    public static void SetBurnStats(float newTickDamage, float newTickInterval, float newDuration)
    {
        tickDamage = newTickDamage;
        tickInterval = newTickInterval;
        duration = newDuration;
    }
}
