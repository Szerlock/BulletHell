using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    [SerializeField] private Animator cutSceneAnimator;
    [SerializeField] private Transform cutScenePosition;
    private float startTime;
    private bool isPaused = false;
    private bool playAnimation = false;


    public override void Init()
    {
        base.Init();
        AudioManager.Instance.PlayBossMusic(3, 1);
        GameManager.Instance.AddEnemy(center);
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        cutSceneAnimator.Play("OpenGate");
        playAnimation = true;
        yield return new WaitForSeconds(5f);
        bossStateHandler.Init(this);
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            if (isInitialized)
            {
                if (currentState == State.Moving && isMoving)
                {
                    RotateWhileMoving();
                }
            }
            else if (playAnimation)
            {
                Vector3 direction = (cutScenePosition.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, cutScenePosition.position, 12 * Time.deltaTime);

                transform.Rotate(Vector3.up * 360f * Time.deltaTime);

                float elapsedTime = Time.time - startTime;
                if (Vector3.Distance(transform.position, cutScenePosition.position) < 0.1f)
                {
                    isInitialized = true;
                    Debug.Log($"Reached the target in {elapsedTime} seconds");
                    playAnimation = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isInitialized && !isPaused)
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

        //isPlayingPose = true;

        //for (int i = 0; i < clipNames.Count - 1; i++)
        //{
        //    animator.Play(clipNames[i]);
        //    yield return new WaitForSeconds(GetClipLength(clipNames[i]));
        //}

        //if (!SecondPhase)
        //    yield return new WaitUntil(() => currentState == State.Waiting);
        //else
        //{
        //    yield return new WaitUntil(() => currentState == State.Attacking);
        //    animator.Play(clipNames[clipNames.Count - 1]);
        //    yield return new WaitForSeconds(GetClipLength(clipNames[clipNames.Count - 1]));
        //    currentPoseCoroutine = StartCoroutine(PlayConjuringSequence());
        //    isPlayingPose = false;
        //    yield break;
        //}

        //string lastClip = clipNames[clipNames.Count - 1];
        //animator.Play(lastClip);
        //yield return new WaitForSeconds(GetClipLength(lastClip));

        //animator.Play(idleStateName);
        //isPlayingPose = false;

        isPlayingPose = true;

        // Play all clips except last, during movement
        for (int i = 0; i < clipNames.Count - 1; i++)
        {
            animator.Play(clipNames[i]);
            yield return new WaitForSeconds(GetClipLength(clipNames[i]));
        }

        if (!SecondPhase)
        {
            // Wait for bullet attack to finish (custom condition maybe?)
            yield return new WaitUntil(() => currentState == State.Waiting);
            yield return new WaitForSeconds(2f); // The required delay after bullet
        }
        else
        {
            // In second phase, play the final clip right away
            animator.Play(clipNames[clipNames.Count - 1]);
            yield return new WaitForSeconds(GetClipLength(clipNames[clipNames.Count - 1]));

            // Begin conjuring
            currentPoseCoroutine = StartCoroutine(PlayConjuringSequence());
            isPlayingPose = false;
            yield break;
        }

        // Only play final clip in first phase after waiting
        animator.Play(clipNames[clipNames.Count - 1]);
        yield return new WaitForSeconds(GetClipLength(clipNames[clipNames.Count - 1]));

        // Return to idle
        animator.Play(idleStateName);
        isPlayingPose = false;
    }

    private IEnumerator PlayConjuringSequence()
    {
        //animator.Play(conjuringClips[0]);
        //yield return new WaitForSeconds(GetClipLength(conjuringClips[1]));

        //animator.Play(conjuringClips[1]);
        //yield return new WaitForSeconds(2f);

        //animator.Play(conjuringClips[2]);
        //yield return new WaitForSeconds(GetClipLength(conjuringClips[2]));
        //isPlayingPose = false;

        //currentPoseCoroutine = StartCoroutine(PlayUnstableSequence());
        animator.Play(conjuringClips[0]);
        yield return new WaitForSeconds(GetClipLength(conjuringClips[0]));

        animator.Play(conjuringClips[1]); // This is the looping one
        yield return new WaitForSeconds(2f); // Maintain the loop for 2 sec

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
        if (!isPlayingPose)
            PlayRandomPoseSequence();
        //PlayRandomPoseSequence();
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
        ChangeBackground.Instance.SwitchVolumes(1);
    }

    protected override void Die()
    {
        GameManager.Instance.ShowEndScreen(true);
    }
}
