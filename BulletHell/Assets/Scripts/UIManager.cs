using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private Animator animator;
    [SerializeField] private string backGround = "AugmentBackground";
    [SerializeField] private List<Animator> augment;
    [SerializeField] private string augment1 = "Augment1Movement";
    [SerializeField] private string augment2 = "Augment2Movement";
    [SerializeField] private string augment3 = "Augment3Movement";

    public bool backgroundUp = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayBackground()
    {
        animator.gameObject.SetActive(true);
        backgroundUp = true;
        animator.Play(backGround, 0, 0);
    }

    public void PlayAugments()
    {
        augment[0].Play(augment1,0, 0);
        augment[0].Update(0);
        augment[1].Play(augment2, 0, 0);
        augment[1].Update(0);
        augment[2].Play(augment3, 0, 0);
        augment[2].Update(0);
    }

    public void EndBackground()
    {
        augment[0].Update(0);
        backgroundUp = false;
        animator.gameObject.SetActive(false);
    }
}
