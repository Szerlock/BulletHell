using System.Collections;
using UnityEngine;

public class Mask : MonoBehaviour
{
    private bool gotHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gotHit) return;
        if (other.CompareTag("PlayerBullet"))
        {
            gotHit = true;
            if(!GameManager.Instance.isOnTutorial)
                BossManager.Instance.currentBoss.isInitialized = false;
            StartCoroutine(BossManager.Instance.augmentManager.StartAugmentPicking(1));
            StartCoroutine(WaitForAugment());
        }
    }

    public IEnumerator WaitForAugment()
    {
        yield return new WaitUntil(() => BossManager.Instance.augmentManager.finishedPickingAugment);
        if (!GameManager.Instance.isOnTutorial)
            BossManager.Instance.currentBoss.isInitialized = true;
        SpawnMask.Instance.masks.Remove(gameObject);
        Destroy(gameObject);
    }
}

