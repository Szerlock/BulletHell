using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBallerinaBoss : BossBase
{
    [SerializeField] private List<BallerinaUnit> ballerinas;
    [SerializeField] private float chainRadius;
    [SerializeField] private float chainExpandSpeed;
    [SerializeField] private float orbitSpeed;
    public bool isFiring = false;

    private float currentRadius;

    [Header("Expand Shrink Radius")]
    [SerializeField] private List<float> danceRadii;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float timeBetweenTransitions;
    private Coroutine danceRoutine;
    [SerializeField] private float minRadiusDifference = 10;

    [Header("AirAttack")]
    [SerializeField] private float airRadiusCooldown;
    [SerializeField] private float airPosition;
    [SerializeField] private float airRadius;

    protected override void Start()
    {
        base.Start();

        bossStateHandler.Init(this);

        currentHealth = Health;
        GameManager.Instance.currentBoss = this;

        currentRadius = chainRadius;

        foreach(BallerinaUnit ballerina in ballerinas)
        {
            ballerina.Init(this);
        }

        StartDanceRoutine();
    }

    private void Update()
    {
        currentRadius = Mathf.Lerp(currentRadius, chainRadius, Time.deltaTime * chainExpandSpeed);

        UpdateBallerinaPositions();
    }

    private void UpdateBallerinaPositions()
    {
        float angleStep = 360f / ballerinas.Count;
        float timeRotation = Time.time * orbitSpeed;

        for (int i = 0; i < ballerinas.Count; i++)
        {
            float angle = (angleStep * i + timeRotation) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * currentRadius;
            ballerinas[i].MoveToPosition(transform.position + offset);
        }
    }

    public void StartDanceRoutine()
    {
        if (danceRoutine != null) StopCoroutine(danceRoutine);
        danceRoutine = StartCoroutine(DanceRoutineCoroutine());
    }

    private IEnumerator DanceRoutineCoroutine()
    {
        while (true)
        {
            float newRadius = danceRadii[Random.Range(0, danceRadii.Count)];

            if (Mathf.Abs(newRadius - Mathf.Abs(currentRadius)) <= minRadiusDifference)
            {
                newRadius *= -1f;
            }
            AnimateChainRadius(newRadius, transitionDuration);
            yield return new WaitForSeconds(Random.Range(5, timeBetweenTransitions));
        }
    }

    public void AnimateChainRadius(float targetRadius, float duration)
    {
        StartCoroutine(AnimateRadiusCoroutine(targetRadius, duration));
    }

    private IEnumerator AnimateRadiusCoroutine(float targetRadius, float duration)
    {
        float startRadius = chainRadius;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            chainRadius = Mathf.Lerp(startRadius, targetRadius, t);
            yield return null;
        }

        chainRadius = targetRadius;
    }

    public float GetChainRadius()
    {
        return currentRadius;
    }

    public void BallerinasFire()
    {
        isFiring = true;
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.bulletSpawner.StartFiring();
        }
    }

    public bool IsBallerinaAttackFinished()
    {
        bool finished = false;
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            finished = ballerina.bulletSpawner.AttackFinished();
        }
        return finished;
    }

    public void ResetAttack()
    {
        isFiring = false;
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.bulletSpawner.ResetAttack();
        }
    }
}
