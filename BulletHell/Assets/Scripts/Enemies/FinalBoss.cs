using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBoss : BossBase
{

    [SerializeField] private List<string> pose1Clips;
    [SerializeField] private List<string> pose2Clips;
    [SerializeField] private List<string> pose3Clips;
    [SerializeField] private string idleStateName = "BallerinaIdle";
    [SerializeField] private List<string> conjuringClips;
    [SerializeField] private List<string> unstableClips;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private RotateFinal rotator;

    private Coroutine currentPoseCoroutine;
    [SerializeField] private Transform center;
    public override void Init()
    {
        base.Init();
        AudioManager.Instance.PlayBossMusic(3, 1);
        GameManager.Instance.AddEnemy(center);
        bossStateHandler.Init(this);
    }

    private void Update()
    {
        if (isInitialized)
        {
            if (currentState == State.Moving && isMoving)
            {
                RotateWhileMoving();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isInitialized)
        {
            baseMoveUpdate();
        }
    }

    private void baseMoveUpdate()
    {
        if (currentState == State.Moving)
        {
            if (!isMoving)
            {
                isMoving = true;
                OnStartMoving();
            }
            if (!hasTarget)
            {
                PickNewTarget();
            }
            MoveToTarget();
        }
    }
    public void RotateWhileMoving()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void PlayRandomPoseSequence()
    {
        if (currentPoseCoroutine != null)
            StopCoroutine(currentPoseCoroutine);
        currentPoseCoroutine = null;
        int poseIndex = UnityEngine.Random.Range(0, 3);

        List<string> chosenPose = poseIndex switch
        {
            0 => pose1Clips,
            1 => pose2Clips,
            2 => pose3Clips,
            _ => throw new System.NotImplementedException(),
        };

        currentPoseCoroutine = StartCoroutine(PlayPoseSequence(chosenPose));
    }

    public void PlayAnimationSequence(string name)
    {
        if (currentPoseCoroutine != null)
            StopCoroutine(currentPoseCoroutine);
        currentPoseCoroutine = null;
        currentPoseCoroutine = name switch
        {
            "Conjuring" => StartCoroutine(PlayConjuringSequence()),
            "Unstable" => StartCoroutine(PlayUnstableSequence()),
            _ => StartCoroutine(PlayPoseSequence(unstableClips)),
        };
    }

    private IEnumerator PlayPoseSequence(List<string> clipNames)
    {

        isPlayingPose = true;

        for (int i = 0; i < clipNames.Count - 1; i++)
        {
            animator.Play(clipNames[i]);
            yield return new WaitForSeconds(GetClipLength(clipNames[i]));
        }

        if (!SecondPhase)
            yield return new WaitUntil(() => currentState == State.Waiting);
        else
        {
            yield return new WaitUntil(() => currentState == State.Attacking);
            animator.Play(clipNames[clipNames.Count - 1]);
            yield return new WaitForSeconds(GetClipLength(clipNames[clipNames.Count - 1]));
            currentPoseCoroutine = StartCoroutine(PlayConjuringSequence());
            yield break;
        }

        string lastClip = clipNames[clipNames.Count - 1];
        animator.Play(lastClip);
        yield return new WaitForSeconds(GetClipLength(lastClip));

        animator.Play(idleStateName);
        isPlayingPose = false;
    }

    private IEnumerator PlayConjuringSequence()
    {
        animator.Play(conjuringClips[0]);
        yield return new WaitForSeconds(GetClipLength(conjuringClips[1]));

        animator.Play(conjuringClips[1]);
        yield return new WaitForSeconds(2f);

        animator.Play(conjuringClips[2]);
        yield return new WaitForSeconds(GetClipLength(conjuringClips[2]));
        isPlayingPose = false;

        currentPoseCoroutine = StartCoroutine(PlayUnstableSequence());
    }

    private IEnumerator PlayUnstableSequence()
    {

        animator.Play(unstableClips[0]);
        yield return new WaitForSeconds(2f);
        animator.Play(unstableClips[1]);

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

    protected override void OnStartMoving()
    {
        PlayRandomPoseSequence();
    }

    public override void TakeDamage(float amount)
    {
        if (!isInitialized) return;
        base.TakeDamage(amount);
        HealthBar.Instance.SetHealth(currentHealth - amount);
        if (currentHealth <= currentHealth/2)
        {
            StartSecondPhase();
        }
    }

    public override void StartSecondPhase()
    {
        SecondPhase = true;
        AudioManager.Instance.PlayBossMusic(3, 2);
        fireCooldown = unstableFireCooldown;
    }
}
