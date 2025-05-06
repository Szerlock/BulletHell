using UnityEngine;
using UnityEngine.Rendering;

public class ChangeBackground : MonoBehaviour
{
    public Volume volume1;
    public Volume volume2;

    void Start()
    {
        volume1.enabled = true;
        volume2.enabled = false;
    }

    [ContextMenu("SwitchToVolume1")]
    public void SwitchToVolume1()
    {
        volume1.enabled = true;
        volume2.enabled = false;
    }

    [ContextMenu("SwitchToVolume2")]
    public void SwitchToVolume2()
    {
        volume1.enabled = false;
        volume2.enabled = true;
    }

}
