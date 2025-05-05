using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownBoss : BossBase
{
    [SerializeField] private List<Transform> rope1;
    [SerializeField] private List<Transform> rope2;
    [SerializeField] private List<Transform> rope3;
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private List<float> damageTracker;
    [SerializeField] private List<string> attackNames;

    protected override void Start()
    {
        base.Start();
        bossStateHandler = new ClownStateHandler();
        bossStateHandler.Init(this);

        currentHealth = Health;
        moveTimer = moveCooldown;
        GameManager.Instance.currentBoss = this;
    }

    private void Update()
    {
        if (currentState == State.Juggling)
        {
            Juggling();
        }
    }

    private void Juggling()
    {
        animator.Play("Juggling");
    }

    public void PickAttack()
    {
        int attackIndex = Random.Range(0, attackNames.Count);
        switch (attackNames[attackIndex])
        {
            case "Bombs":
                ThrowBombs();
                break;
            case "Box":
                HideInBox();
                break;
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if(currentHealth <= damageTracker[0])
        {
            damageTracker.RemoveAt(0);
            StartCoroutine(PlayAnimation("Clown Falling"));
        }
        if (currentHealth <= currentHealth / 2)
        {
            StartSecondPhase();
        }
    }

    protected override void StartSecondPhase()
    {
        SecondPhase = true;
        fireCooldown = unstableFireCooldown;
    }

    private IEnumerator PlayAnimation(string name)
    {
        animator.Play(name);
        yield return new WaitForSeconds(GetClipLength(name));
        // Eventually play puff vfx
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
