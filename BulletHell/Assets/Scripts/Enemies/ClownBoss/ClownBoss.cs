using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

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

    [Header("Unstable")]
    [SerializeField] private ChangeMaterial changeMaterial;

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
    [SerializeField] private Animator hammerAnimator;
    [SerializeField] private GameObject hammer;
    public bool boxesUntargetable = false;


    [Header("RotateBox Variables")]
    public List<Transform> airPositions;
    public List<Transform> boxList;
    public bool isShuffling = false;
    public float shuffleDuration;
    public float waitingTime;
    public float moveDuration = 1f;
    public List<Vector3> rotationBoxes;


    [Header("Movement Variables")]
    [SerializeField] private Transform currentStartTransform; 
    [SerializeField] private Transform currentEndTransform;   
    private bool movingToEnd = true;
    private bool ropePicked = true;

    [Header("Conjuring Variables")]
    public List<Transform> decoySpots;
    public GameObject decoyPrefab;
    public List<GameObject> bossDecoys;
    public Transform decoyTarget;

    [SerializeField] private Transform center;
    [SerializeField] private ParticleSystem boxPoof;
    [SerializeField] private ParticleSystem poof;


    public bool isInCinematic;
    [SerializeField] private Transform cinematicMoveTowards;
    private bool hasFallen = false;

    public override void Init()
    {
        base.Init();
        AudioManager.Instance.PlayBossMusic(1, 1);
        GameManager.Instance.AddEnemy(center);

        CameraFollow.Instance.EnterCinematic(0);
        StartCoroutine(Cinematic());

    }

    private IEnumerator Cinematic()
    {
        isInCinematic = true;
        isInitialized = true;
        bossStateHandler.Init(this);
        yield return new WaitForSeconds(3f);
        isInCinematic = false;
        CameraFollow.Instance.ExitCinematic();
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            if (isInitialized)
            {
                if (currentState == State.Juggling)
                {
                    ReturnToNormal();
                    Juggling();
                }
                else
                {
                    ropePicked = false;
                }

                if (isConjuring && bossDecoys.Count == 0)
                {
                    isAttacking = false;
                    isConjuring = false;
                    hasFallen = false; 
                    if(!SecondPhase)
                    AudioManager.Instance.PlayBossMusic(1, 1);
                    ChangeBackground.Instance.SwitchVolumes(1);
                }
            }
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

        poof.Play();
        AudioManager.Instance.PlaySFX("PoofClown");
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
        boxesUntargetable = true;
        isHiding = true;

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
        isShuffling = true;

        yield return new WaitForSeconds(GetClipLength("Clown Reveal"));
        transform.localScale = new Vector3(0f, 0f, 0f);

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
        boxesUntargetable = false;
    }

    public IEnumerator HammerSlam()
    {
        hammer.SetActive(true);
        ReturnToNormal();
        boxAnimator.Play("Box Hammer");
        animator.Play("Clown Hammer");
        hammerAnimator.Play("Hammer Slam");
        yield return new WaitForSeconds(GetClipLength("Clown Hammer"));
        GameManager.Instance.Player.TakeDamage(Damage);
        GameManager.Instance.Player.PushPlayer();
        yield return new WaitForSeconds(lingerTime);
        // Play VFX
        hammer.SetActive(false);
        isAttacking = false;
        isHiding = false;
        RemoveBoxes();
    }

    public IEnumerator Stunned()
    {
        ReturnToNormal();
        boxAnimator.Play("Box Hammer");
        animator.Play("Clown Stunned");
        yield return new WaitForSeconds(stunDuration);
        // Play VFX
        TakeDamage(GameManager.Instance.Player.damage);
        isAttacking = false;
        isHiding = false;
        RemoveBoxes();
    }

    public void StartConjuring()
    {
        isConjuring = true;
        if (!SecondPhase)
        {
            ChangeBackground.Instance.SwitchVolumes(1);
            AudioManager.Instance.PlayBossMusic(1, 2);
        }
        StartCoroutine(SpawnDecoys());
    }

    public IEnumerator SpawnDecoys()
    {
        isAttacking = true;
        for (int i = 0; i < 5; i++)
        {
            GameObject decoy = Instantiate(decoyPrefab, decoySpots[i].position, decoySpots[i].rotation);
            decoy.transform.LookAt(decoyTarget);
            bossDecoys.Add(decoy);

            if (decoy.TryGetComponent(out Animator animator))
            {
                animator.Play("Clown Conjuring");
            }
        }

        yield return new WaitForSeconds(GetClipLength("Clown Conjuring"));

        foreach(var decoy in bossDecoys)
        {
            if (decoy.TryGetComponent(out BossDecoy bossDecoy))
            {
                bossDecoy.target = decoyTarget;
                bossDecoy.Init();
            }
        }
    }

    private void ReturnToNormal()
    {
        transform.SetParent(null);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void RemoveBoxes()
    {
        //foreach (Transform box in boxList)
        //{
        //    Instantiate(boxPoof.gameObject, box.position, Quaternion.identity);
        //    boxPoof.Play();
        //    Destroy(box.gameObject);
        //}
        //boxList.Clear();

        foreach (Transform box in boxList)
        {
            GameObject poofParent = new GameObject("PoofEffect");
            poofParent.transform.position = box.position;

            ParticleSystem ps = Instantiate(boxPoof, poofParent.transform);
            ps.transform.localPosition = Vector3.zero;
            ps.Play();

            Destroy(poofParent, ps.main.duration + ps.main.startLifetime.constantMax);
            Destroy(box.gameObject);
        }

        boxList.Clear();
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
            Vector3 lookTarget = new Vector3(player.position.x, obj.position.y, player.position.z);
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
        //for (int i = 0; i < TimesToThrow; i++)
        //{
        //    List<Transform> tempList = new List<Transform>(bombTargets);
        //    bombsThrown = 0;

        //    animator.Play("ThrowBombs", -1, 0f);
        //    AudioManager.Instance.PlaySFX("Bombing");
        //    transform.LookAt(player);
        //    yield return new WaitForSeconds(1f);
        //    while (bombsThrown < bombCount)
        //    {

        //        int randomIndex = Random.Range(0, tempList.Count);

        //        Transform selectedPosition = tempList[randomIndex];
        //        tempList.RemoveAt(randomIndex);

        //        //GameObject bomb = Instantiate(Bomb, spawnBombLoc.position, Quaternion.identity);

        //        GameObject bomb = BombsPool.Instance.GetBomb(spawnBombLoc.position, Quaternion.identity);
        //        if (bomb != null)
        //        {
        //            Bomb bombScript = bomb.GetComponent<Bomb>();
        //            bombScript.Init(selectedPosition);
        //            bombsThrown++;
        //        }

        //        //Bomb bombScript = bomb.GetComponent<Bomb>();
        //        //bombScript.Init(selectedPosition);
        //        //bombsThrown++;
        //    }

        //    yield return new WaitForSeconds(WaitingTimeBomb);
        //}
        //isAttacking = false;
        //ropePicked = false;

        for (int i = 0; i < TimesToThrow; i++)
        {
            bombsThrown = 0;

            animator.Play("ThrowBombs", -1, 0f);
            AudioManager.Instance.PlaySFX("Bombing");
            transform.LookAt(player);
            yield return new WaitForSeconds(1f);

            foreach (Transform target in bombTargets)
            {
                GameObject bomb = BombsPool.Instance.GetBomb(spawnBombLoc.position, Quaternion.identity);
                if (bomb != null)
                {
                    Bomb bombScript = bomb.GetComponent<Bomb>();
                    bombScript.Init(target);
                    bombsThrown++;
                }

                if (bombsThrown >= bombCount)
                    break;

            }

            yield return new WaitForSeconds(WaitingTimeBomb);
        }

        isAttacking = false;
        ropePicked = false;
    }

    public override void TakeDamage(float amount)
    {
        if (!isInitialized) return;
        if (isConjuring) return;
        HealthBar.Instance.SetHealth(currentHealth - amount);
        base.TakeDamage(amount);
        if (damageTracker.Count > 0 && currentHealth <= damageTracker[0])
        {
            damageTracker.RemoveAt(0);
            Fall();
        }
        if (currentHealth <= Health / 2 && !SecondPhase)
        {
            StartSecondPhase();
        }
    }

    private void Fall()
    {
        if (hasFallen) return;
        hasFallen = true;

        StopAllCoroutines();

        transform.LookAt(player);
        currentState = State.Falling;
        StartCoroutine(PlayAnimation("Clown Falling"));
    }

    [ContextMenu("StartSecondPhase")]
    public override void StartSecondPhase()
    {
        ChangeBackground.Instance.SwitchVolumes(1);
        SecondPhase = true;
        fireCooldown = unstableFireCooldown;
        AudioManager.Instance.PlayBossMusic(1, 2);

        changeMaterial.ChangeMat(true);
    }

    private IEnumerator PlayAnimation(string name)
    {
        currentState = State.Falling;
        animator.Play(name);
        yield return new WaitForSeconds(.5f);
        poof.Play();
        AudioManager.Instance.PlaySFX("PoofClown");
        yield return new WaitForSeconds(.5f);
        currentState = State.Conjuring;
        transform.localScale = Vector3.zero;
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

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;
        if (boxList.Count > 0)
        {
            RemoveBoxes();
        }


        base.Die();
    }
}
