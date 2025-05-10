using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private Animator animator;
    [SerializeField] private string backGround = "AugmentBackground";
    [SerializeField] private List<Animator> augment;
    [SerializeField] private string augment1 = "Augment1Movement";
    [SerializeField] private string augment2 = "Augment2Movement";
    [SerializeField] private string augment3 = "Augment3Movement";

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
        animator.Play(backGround);
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

    private float GetClipLength(string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.LogWarning($"Clip '{clipName}' not found in Animator.");
        return 10;
    }
}
