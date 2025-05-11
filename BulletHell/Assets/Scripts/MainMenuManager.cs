using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> backgroundImages;
    [SerializeField] private Image background;

    void Start()
    {
        StartCoroutine(ChangeBackground());
    }

    private IEnumerator ChangeBackground()
    {
        int index = 0;
        while (true)
        {
            background.sprite = backgroundImages[index];
            index = (index + 1) % backgroundImages.Count;
            yield return new WaitForSeconds(5f);
        }
    }

    public void ChanceScene()
    {
        SceneManager.LoadScene("Arena");
    }
}
