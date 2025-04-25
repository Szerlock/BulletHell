using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilZone : MonoBehaviour
{
    public GameObject fireEffectPrefab;
    private bool isIgnited = false;
    private HashSet<EnemyBase> burnedEnemies = new HashSet<EnemyBase>();

    private BurnEffect burnEffect;

    private void Awake()
    {
        burnEffect = GlobalBurnStats.GetBurnEffect();
    }

    public void Ignite()
    {
        if (isIgnited) return;

        isIgnited = true;

        //if (fireEffectPrefab != null)
        //    Instantiate(fireEffectPrefab, transform.position, Quaternion.identity, transform);

        StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        float burnTime = 0f;
        while (burnTime < burnEffect.duration)
        {
            burnTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isIgnited && other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && !burnedEnemies.Contains(enemy))
            {
                burnedEnemies.Add(enemy);
                enemy.ApplyOilBurn(burnEffect.tickDamage, burnEffect.tickInterval, burnEffect.duration);
            }
        }
    }
}
