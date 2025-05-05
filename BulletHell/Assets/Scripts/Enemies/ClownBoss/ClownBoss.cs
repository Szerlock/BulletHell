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
    [SerializeField] private List<GameObject> balls;  


    [Header("Bomb Attack Variables")]
    [SerializeField] private List<Transform> bombTargets;
    [SerializeField] private GameObject Bomb;
    [SerializeField] private Transform spawnBombLoc;
    public int bombCount;
    private int bombsThrown;
    public int TimesToThrow;
    public float WaitingTimeBomb;

    [Header("HideInBox Attack Variables")]
    [SerializeField] private List<Transform> boxPositions;
    [SerializeField] private GameObject Box;
    [SerializeField] private Vector3 spawnOffsetRelativeToBox = new Vector3(0, -4, -1.5f);

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
            ShowBalls();
        }
        else
            MoveBetweenRopePoints();
    }

    private void PickRope()
    {
        int ropeIndex = Random.Range(0, 3);

        switch (ropeIndex)
        {
            case 0:
                currentStartTransform = rope1[0];
                currentEndTransform = rope1[1];
                transform.position = rope1[2].position;
                break;
            case 1:
                currentStartTransform = rope2[0];
                currentEndTransform = rope2[1];
                transform.position = rope2[2].position;
                break;
            case 2:
                currentStartTransform = rope3[0];
                currentEndTransform = rope3[1];
                transform.position = rope3[2].position;
                break;
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
            transform.LookAt(targetTransform);

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
        HideBalls();

        foreach (Transform box in boxPositions)
        {
            GameObject spawnedBox = Instantiate(Box, box.position + new Vector3(0, 7, 0), Quaternion.identity);
            Vector3 lookTarget = new Vector3(player.position.x, spawnedBox.transform.position.y, player.position.z);
            spawnedBox.transform.LookAt(lookTarget);
        }

        int index = Random.Range(0, boxPositions.Count);
        Transform chosenBox = boxPositions[index];


        animator.Play("Clown Reveal");
        transform.parent = chosenBox;
        transform.position = spawnOffsetRelativeToBox;

        Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.transform.LookAt(target);
    }

    private void OnDrawGizmos()
    {
        if (boxPositions == null || boxPositions.Count == 0) return;

        foreach (Transform box in boxPositions)
        {
            Vector3 offsetPos = box.position + box.TransformDirection(spawnOffsetRelativeToBox);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(offsetPos, 0.25f); 

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(box.position, offsetPos); 

            if (player != null)
            {
                Vector3 faceDir = player.position - offsetPos;
                faceDir.y = 0;
                Gizmos.color = Color.red;
                Gizmos.DrawRay(offsetPos, faceDir.normalized * 1.5f);
            }
        }
    }

    private IEnumerator ThrowBombs()
    {
        for (int i = 0; i < TimesToThrow; i++)
        {
            List<Transform> tempList = new List<Transform>(bombTargets);
            bombsThrown = 0;

            animator.Play("ThrowBombs", -1, 0f);
            transform.LookAt(player);
            yield return new WaitForSeconds(1f);
            while (bombsThrown < bombCount)
            {

                int randomIndex = Random.Range(0, tempList.Count);

                Transform selectedPosition = tempList[randomIndex];
                tempList.RemoveAt(randomIndex);

                GameObject bomb = Instantiate(Bomb, spawnBombLoc.position, Quaternion.identity);

                Bomb bombScript = bomb.GetComponent<Bomb>();
                bombScript.Init(selectedPosition);
                bombsThrown++;
            }

            yield return new WaitForSeconds(WaitingTimeBomb);
        }
        isAttacking = false;
        ropePicked = false;
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

    private void ShowBalls()
    {
        foreach (GameObject ball in balls)
        {
            ball.SetActive(true);
        }
    }

    private void HideBalls()
    {
        foreach (GameObject ball in balls)
        {
            ball.SetActive(false);
        }
    }
}
