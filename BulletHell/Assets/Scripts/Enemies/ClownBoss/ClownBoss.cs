using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ClownBoss : BossBase
{
    [SerializeField] private List<Transform> rope1;
    [SerializeField] private List<Transform> rope2;
    [SerializeField] private List<Transform> rope3;
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private GameObject chosenBox;
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
    [SerializeField] private float stunDuration;
    [SerializeField] private float lingerTime;

    [Header("RotateBox Variables")]
    public List<Transform> airPositions;
    public List<Transform> boxList;
    public bool isShuffling = false;
    public float shuffleDuration;
    public float waitingTime;
    public float moveDuration = 1f;
    public List<Vector3> rotationBoxes;



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

        List<GameObject> boxes = new List<GameObject>();

        foreach (Transform box in boxPositions)
        {
            GameObject spawnedBox = Instantiate(Box, box.position + new Vector3(0, 7, 0), Quaternion.identity);
            Vector3 lookTarget = new Vector3(player.position.x, spawnedBox.transform.position.y, player.position.z);
            spawnedBox.transform.LookAt(lookTarget);
            spawnedBox.GetComponent<Box>().clownBoss = this;
            boxes.Add(spawnedBox);
            boxList.Add(spawnedBox.transform);
            rotationBoxes.Add(lookTarget);
        }

        int index = Random.Range(0, boxPositions.Count);
        chosenBox = boxes[index];
        chosenBox.GetComponent<Box>().ChoosenBox = true;


        boxAnimator = chosenBox.GetComponent<Animator>();
        boxAnimator.Play("Box Reveal"); 
        animator.Play("Clown Reveal");
        
        transform.parent = chosenBox.transform;
        transform.localPosition = spawnOffsetRelativeToBox;

        Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.transform.LookAt(target);
        StartShuffle();
    }

    public void StartShuffle()
    {
        StartCoroutine(ShuffleBoxes());
    }

    private IEnumerator ShuffleBoxes()
    {
        yield return new WaitForSeconds(GetClipLength("Clown Reveal"));
        isShuffling = true;

        // Spin Boxes
        for (int i = 0; i < boxList.Count; i++)
        {
            StartCoroutine(MoveToPosition(boxList[i], airPositions[i].position, moveDuration, false));
            StartCoroutine(SpinBox(boxList[i]));
        }
        yield return new WaitForSeconds(moveDuration + 0.1f);

        float elapsed = 0f;
        while (elapsed < shuffleDuration)
        {
            int a = Random.Range(0, boxList.Count);
            int b = Random.Range(0, boxList.Count);
            if (a != b)
            {
                Vector3 temp = boxList[a].position;
                StartCoroutine(MoveToPosition(boxList[a], boxList[b].position, 0.3f, false));
                StartCoroutine(MoveToPosition(boxList[b], temp, 0.3f, false));
            }

            elapsed += waitingTime;
            yield return new WaitForSeconds(waitingTime);
        }
        isShuffling = false;

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < boxPositions.Count; i++) availableIndices.Add(i);

        for (int i = 0; i < boxList.Count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int chosenGroundIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            Transform box = boxList[i];
            Vector3 targetPos = boxPositions[chosenGroundIndex].position + new Vector3(0, 7, 0);
            StartCoroutine(MoveToPosition(box, targetPos, moveDuration, true));
        }

        yield return new WaitForSeconds(moveDuration + 0.1f);

    }

    public IEnumerator HammerSlam()
    {
        boxAnimator.Play("Box Hammer");
        animator.Play("Clown Hammer");
        yield return new WaitForSeconds(GetClipLength("Clown Hammer"));
        GameManager.Instance.Player.TakeDamage(Damage);
        yield return new WaitForSeconds(lingerTime);
        // Play VFX
        isAttacking = false;
        transform.SetParent(null);
    }

    public IEnumerator Stunned()
    {
        boxAnimator.Play("Box Hammer");
        animator.Play("Clown Stunned");
        yield return new WaitForSeconds(stunDuration);
        // Play VFX
        TakeDamage(GameManager.Instance.Player.damage);
        isAttacking = false;
        transform.SetParent(null);
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 target, float duration, bool rotateToPlayer)
    {
        Vector3 start = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = target;

        if (rotateToPlayer)
        {
            Quaternion startRot = obj.rotation;
            Vector3 lookTarget = new Vector3(0, obj.position.y, player.position.z);
            Quaternion targetRot = Quaternion.LookRotation(lookTarget - obj.position);

            float t = 0f;
            float rotDuration = 0.2f;

            while (t < rotDuration)
            {
                obj.rotation = Quaternion.Slerp(startRot, targetRot, t / rotDuration);
                t += Time.deltaTime;
                yield return null;
            }

            obj.rotation = targetRot;
        }
    }

    private IEnumerator SpinBox(Transform box)
    {
        float time = 0f;
        Vector3 spinSpeed = new Vector3(
            Random.Range(90f, 180f),
            Random.Range(90f, 180f),
            Random.Range(90f, 180f)
        );

        while (isShuffling)
        {
            box.Rotate(spinSpeed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
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
