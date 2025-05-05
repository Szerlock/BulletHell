using NUnit.Framework;
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


    [Header("Bomb Attack Variables")]
    [SerializeField] private List<Transform> bombTargets;
    [SerializeField] private GameObject Bomb;
    public int bombCount;
    private int bombsThrown;
    public int TimesToThrow;
    public float WaitingTimeBomb;

    [Header("Movement Variables")]
    private Transform currentStartTransform; 
    private Transform currentEndTransform;   
    private bool movingToEnd = true;
    private bool ropePicked;

    protected override void Start()
    {
        base.Start();
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
        else
        {
            ropePicked = false;
        }
    }

    private void Juggling()
    {
        animator.Play("Clown Juggling");
        if (!ropePicked)
        {
            PickRope();
        }
        else
            MoveBetweenRopePoints();
    }

    private void PickRope()
    {
        int ropeIndex = Random.Range(0, 3);

        switch (0)
        {
            case 0:
                currentStartTransform = rope1[0];
                currentEndTransform = rope1[1];
                break;
            //case 1:
            //    currentStartTransform = rope2[0];
            //    currentEndTransform = rope2[1];
            //    break;
            ////case 2:
            //    currentStartTransform = rope3[0];
            //    currentEndTransform = rope3[1];
            //    break;
        }
        ropePicked = true;
    }

    private void MoveBetweenRopePoints()
    {
        if (currentStartTransform != null && currentEndTransform != null)
        {
            Transform targetTransform = movingToEnd ? currentEndTransform : currentStartTransform;
            float step = speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, step);

            if (Vector3.Distance(transform.position, targetTransform.position) < 0.1f)
            {
                movingToEnd = !movingToEnd;
            }
        }
    }

    public void PickAttack()
    {
        isAttacking = true;
        int attackIndex = Random.Range(0, attackNames.Count);
        switch (attackNames[attackIndex])
        {
            case "Bombs":
                StartCoroutine(ThrowBombs());
                break;
            case "Box":
                HideInBox();
                break;
        }
    }

    private void HideInBox()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator ThrowBombs()
    {
        for (int i = 0; i < TimesToThrow; i++)
        {
            List<Transform> tempList = new List<Transform>(bombTargets);
            bombsThrown = 0;

            while (bombsThrown < bombCount)
            {

                int randomIndex = Random.Range(0, tempList.Count);

                Transform selectedPosition = tempList[randomIndex];
                tempList.RemoveAt(randomIndex);

                GameObject bomb = Instantiate(Bomb, transform.position, Quaternion.identity);

                Bomb bombScript = bomb.GetComponent<Bomb>();
                bombScript.Init(selectedPosition);
                bombsThrown++;
            }

            yield return new WaitForSeconds(WaitingTimeBomb);
        }
        isAttacking = false;
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
