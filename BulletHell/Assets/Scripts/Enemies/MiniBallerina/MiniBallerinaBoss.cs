using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float circleRadius;
    [SerializeField] private Vector3 circleCenter;

    [Header("MoveTowards Places")]
    [SerializeField] private List<Transform> ballerinaStartPositions;
    [SerializeField] private float delayBetweenMovements;
    private Coroutine cinematicEntranceRoutine;

    public override void Init()
    {
        base.Init();
        bossStateHandler.Init(this);

        currentHealth = Health;

        bulletSpawner.boss = this;
        currentRadius = chainRadius;

        float miniHealth = currentHealth / 5;

        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.Init(this, miniHealth);
        }
        timerAir = airAttackCooldown;

        AudioManager.Instance.PlayBossMusic(1, 1);

        StartCoroutine(CinematicEntranceCoroutine());
        //StartDanceRoutine();
    }

    private IEnumerator CinematicEntranceCoroutine()
    {
        for (int i = 0; i < ballerinas.Count; i++)
        {
            BallerinaUnit ballerina = ballerinas[i];
            Vector3 target = ballerinaStartPositions[i].position;

            ballerina.MoveToPosition(target);
            yield return new WaitUntil(() => ballerina.HasReachedTarget());

            yield return new WaitForSeconds(delayBetweenMovements);
        }

        isInitialized = true;
        StartDanceRoutine();
    }

    private void Update()
    {
        if (isInitialized)
        {
            currentRadius = Mathf.Lerp(currentRadius, chainRadius, Time.deltaTime * chainExpandSpeed);

            if (!isAttacking)
                timerAir -= Time.deltaTime;
            if (timerAir <= 0 && isUnstable)
            {
                PerformAirAttack();
            }

            if (!isUnstable)
                UpdateBallerinaPositions();
        }
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
        HealthBar.Instance.SetHealth(currentHealth - amount);
        currentHealth -= amount;
        if (currentHealth < Health / 2 && !isUnstable)
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
        isUnstable = true;
        StopAllCoroutines();
        foreach (BallerinaUnit ballerina in ballerinas)
        {
            ballerina.changeMaterial.ChangeMat(true);
        }
        AudioManager.Instance.PlayBossMusic(1, 2);

        StartCoroutine(RunAroundBox());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        int segments = 64;
        float angleStep = 360f / segments;
        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = new Vector3(
                Mathf.Cos(angle) * circleRadius,
                0f,
                Mathf.Sin(angle) * circleRadius
            ) + circleCenter;

            if (i > 0)
                Gizmos.DrawLine(prevPoint, newPoint);

            prevPoint = newPoint;
        }

        Gizmos.color = Color.red;
        Vector3 randomPoint = GetRandomPointInCircle();
        Gizmos.DrawSphere(randomPoint, 0.3f);
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
        while (isUnstable)
        {
            for (int i = 0; i < ballerinas.Count; i++)
            {
                Vector3 nextPos = GetRandomPointInCircle();
                ballerinas[i].MoveToPosition(nextPos);
            }
            yield return new WaitUntil(() => ballerinas.All(b => b.HasReachedTarget()));
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    private Vector3 GetRandomPointInCircle()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float radius = Mathf.Sqrt(Random.Range(0f, 1f)) * circleRadius; 
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(circleCenter.x + x, circleCenter.y, circleCenter.z + z);
    }

    public void RemoveBallerina(BallerinaUnit miniBal)
    {
        if (ballerinas.Contains(miniBal))
        {
            ballerinas.Remove(miniBal);
        }
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.AllEnemies.Clear();
    }
}
