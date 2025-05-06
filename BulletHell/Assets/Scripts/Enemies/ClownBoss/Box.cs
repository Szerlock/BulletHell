using UnityEngine;

public class Box : MonoBehaviour
{
    public bool ChoosenBox = false;
    public ClownBoss clownBoss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            if (clownBoss.boxesUntargetable)
                return;
            if (ChoosenBox)
            {
                StartCoroutine(clownBoss.Stunned());
            }
            else 
            {
                StartCoroutine(clownBoss.HammerSlam());
            }
        }
    }
}
