using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : BossBase
{

    [SerializeField] private List<string> pose1Clips;
    [SerializeField] private List<string> pose2Clips;
    [SerializeField] private List<string> pose3Clips;
    [SerializeField] private string idleStateName = "BallerinaIdle";

    [SerializeField] private float rotationSpeed;
    [SerializeField] private RotateFinal rotator;

    private void Update()
    {
        if (currentState == State.Moving && isMoving)
        {
            RotateWhileMoving();
        }
    }

    private void FixedUpdate()
    {
        baseMoveUpdate();
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
        int poseIndex = UnityEngine.Random.Range(0, 3);

        List<string> chosenPose = poseIndex switch
        {
            0 => pose1Clips,
            1 => pose2Clips,
            2 => pose3Clips,
        };

        StartCoroutine(PlayPoseSequence(chosenPose));
    }

    private IEnumerator PlayPoseSequence(List<string> clipNames)
    {
        for (int i = 0; i < clipNames.Count - 1; i++)
        {
            animator.Play(clipNames[i]);
            yield return new WaitForSeconds(GetClipLength(clipNames[i]));
        }
        yield return new WaitUntil(() => currentState == State.Waiting);

        string lastClip = clipNames[clipNames.Count - 1];
        animator.Play(lastClip);
        yield return new WaitForSeconds(GetClipLength(lastClip));

        animator.Play(idleStateName);
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
}
