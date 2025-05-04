using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossBase : EnemyBase
{
    [SerializeField] private List<Transform> MovePositions;
    [SerializeField] public BulletSpawner bulletSpawner;
    [SerializeField] public float fireRate;
    [SerializeField] public float fireCooldown;
    [SerializeField] public float unstableFireCooldown;
    [SerializeField] public Transform player;
    public bool isPlayingPose = false;
    public bool isAttacking = false;


    [SerializeField] BossStateHandler bossStateHandler;

    [Header("Moving Settings")]
    [SerializeField] public float moveCooldown;
    [SerializeField] private float moveTimer;
    private Vector3 targetPosition;
    public bool isMoving = false;
    public bool hasTarget = false;

    [Header("SecondPhase")]
    public bool isUnstable = false;
    public bool isConjuring = false;
    public bool SecondPhase = false;

    public State currentState = State.Waiting;

    public enum State
    {
        Waiting,
        Moving,
        Attacking,
        AttackAgain,
        Death,
    }

    protected override void Start()
    {
        base.Start();
        bossStateHandler.Init(this);
        currentHealth = Health;
        moveTimer = moveCooldown;
        GameManager.Instance.currentBoss = this;
        //bossStateHandler.HandleWaiting();
    }

    private void FixedUpdate()
    {
        if (currentState == State.Moving)
        {
            isMoving = true;
            if (!hasTarget)
                PickNewTarget();
            MoveToTarget();
        }
    }

    public void PickNewTarget()
    {
        if (MovePositions.Count <= 1)
            return;

        Vector3 currentPos = transform.position;
        Transform newPos;
        do
        {
            newPos = MovePositions[UnityEngine.Random.Range(0, MovePositions.Count)];
        } while (Vector3.Distance(newPos.position, currentPos) < 0.1f);

        targetPosition = newPos.position;
        hasTarget = true;
    }

    public void MoveToTarget()
    {
        if (!isMoving)
            return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            isMoving = false;
            LookAtPlayer();
        }
    }
    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPos);
        }
    }
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }
    protected virtual void OnStartMoving() { }

    protected virtual void StartSecondPhase() { }
    //public override void OnEnable()
    //{
    //    GameManager.Instance.currentBoss = this;
    //}
}
