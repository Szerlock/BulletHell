using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeBackground : MonoBehaviour
{
    [SerializeField] private Volume happyVol, evilVol;
    private bool switched = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SwitchVolumes(1f);
    }

    public void SwitchVolumes(float speed) // CALL THIS WHENEVER YOU WANT TO SWITCH BETWEEN VOLUMES ( BUT THE PREVIOUS SWITCH HAS TO BE COMPLETED )
    {
        if (!switched) return;

        StartCoroutine(SwitchVolumes(speed, happyVol.weight <= 0));
        switched = false;
    }

    private IEnumerator SwitchVolumes(float speed, bool switchToHappyVol)
    {
        Volume selectedVol = switchToHappyVol ? happyVol : evilVol;
        Volume notSelectedVol = !switchToHappyVol ? happyVol : evilVol;

        while (selectedVol.weight < 1)
        {
            selectedVol.weight += Time.deltaTime * speed;
            notSelectedVol.weight -= Time.deltaTime * speed;
            yield return null;
        }

        selectedVol.weight = 1;
        notSelectedVol.weight = 0;

        switched = true;
        print("SWITCH WAS SUCESSFULL");
    }

}
