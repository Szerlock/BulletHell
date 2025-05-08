using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MiniBallerinaBoss : BossBase
{
    [SerializeField] private List<BallerinaUnit> ballerinas;
    [SerializeField] private float chainRadius;
    [SerializeField] private float chainExpandSpeed;
    [SerializeField] private float orbitSpeed;
    public bool isFiring = false;
    [SerializeField] private ChangeMaterial changeMaterial;

    private float currentRadius;

    [Header("Expand Shrink Radius")]
    [SerializeField] private List<float> danceRadii;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float timeBetweenTransitions;
    private Coroutine danceRoutine;
    [SerializeField] private float minRadiusDifference = 10;

    [Header("AirAttack")]
    [SerializeField] private float airAttackCooldown;
    [SerializeField] private Vector3 airPosition;
    [SerializeField] private float airRadius;
    [SerializeField] private float airTransitionDuration;
    [SerializeField] private float timerAir;

    [Header("Unstable")]
    [SerializeField] private Vector3 boxCenter;
    [SerializeField] private Vector3 boxSize;

    protected override void Start()
    {
        base.Start();

        bossStateHandler.Init(this);

        currentHealth = Health;
        GameManager.Instance.currentBoss = this;

        currentRadius = chainRadius;

        float miniHealth = currentHealth / 5;

        foreach(BallerinaUnit ballerina in ballerinas)
        {
            ballerina.Init(this, miniHealth);
        }
        timerAir = airAttackCooldown;
        StartDanceRoutine();
    }

    private void Update()
    {
        currentRadius = Mathf.Lerp(currentRadius, chainRadius, Time.deltaTime * chainExpandSpeed);

        if(!isAttacking)
        timerAir -= Time.deltaTime;
        if (timerAir <= 0 && currentState != State.Unstable)
        {
            PerformAirAttack();
        }

        if(currentState != State.Unstable)
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
        bulletSpawner.PickNewPattern();
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.bulletSpawner.SetPattern(bulletSpawner.currentPattern);
            ballerina.bulletSpawner.Fire();
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
    public void PerformAirAttack()
    {
        timerAir = airAttackCooldown;
        isAttacking = true;
        currentState = State.Conjuring;
        StopAllCoroutines();
        float chainRadius;
        if (currentRadius < 0)
            chainRadius = -airRadius;
        else
            chainRadius = airRadius;
        AnimateChainRadius(chainRadius, transitionDuration);
        StartCoroutine(ConjuringSequence());
    }

    private IEnumerator ConjuringSequence()
    {

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 airPos = airPosition;

        while (elapsed < airTransitionDuration)
        {
            transform.position = Vector3.Lerp(startPos, airPos, elapsed / airTransitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = airPos;
        
        bulletSpawner.StartFiring();
        yield return new WaitUntil(() => bulletSpawner.AttackFinished() == true);
        currentState = State.Waiting;

        elapsed = 0f;

        while (elapsed < airTransitionDuration)
        {
            transform.position = Vector3.Lerp(airPos, startPos, elapsed / airTransitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
        bulletSpawner.ResetAttack();
        isAttacking = false;

        StartDanceRoutine();
    }

    public override void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < Health / 2)
            StartSecondPhase();
        if (currentHealth < 0f)
        {
            Die();
        }
    }

    [ContextMenu("Second Phase")]
    public override void StartSecondPhase()
    {
        currentState = State.Unstable;
        StopAllCoroutines();
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.changeMaterial.ChangeMat(true);
        }
        StartCoroutine(RunAroundBox());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    public void UnstableAttack()
    {
        isFiring = true;
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.bulletSpawner.StartFiring();
        }
    }

    private IEnumerator RunAroundBox()
    {
        while (currentState == State.Unstable)
        {
            for (int i = 0; i < ballerinas.Count; i++)
            {
                Vector3 nextPos = GetRandomPointInBox();
                ballerinas[i].MoveToPosition(nextPos);
                yield return new WaitUntil(() => ballerinas[i].HasReachedTarget());
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            }
        }
    }

    private Vector3 GetRandomPointInBox()
    {
        Vector3 halfSize = boxSize * 0.5f;
        return new Vector3(
            Random.Range(boxCenter.x - halfSize.x, boxCenter.x + halfSize.x),
            boxCenter.y,
            Random.Range(boxCenter.z - halfSize.z, boxCenter.z + halfSize.z)
        );
    }
}
