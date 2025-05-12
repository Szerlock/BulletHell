using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Transform> AllEnemies = new List<Transform>();
    //public BossBase currentBoss;
    public CharacterController3D Player;
    public bool isInCinematic = false;

    [Header("GameEnd Screen0")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject loseBackground;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float wordDelay = 0.2f;
    [SerializeField] private GameObject endScreenButton;
    [SerializeField] private Animator textAnimator;


    [SerializeField] private List<string> lossMessages;

    [SerializeField] private List<string> winMessages;

    public bool isOnTutorial = false;

    [SerializeField] private GameObject tutorial;


    private void Awake()
    {
        Instance = this;
        StartCoroutine(Tutorial());
    }

    private IEnumerator Tutorial()
    {
        textAnimator.Play("TutorialTextAnim", 0, 0);
        isOnTutorial = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIManager.Instance.PlayBackground();
        tutorial.SetActive(true);
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        tutorial.SetActive(false);
        isOnTutorial = false;
        UIManager.Instance.EndBackground();
        StartCoroutine(BossManager.Instance.NextBoss());
        textAnimator.gameObject.SetActive(false);
    }

    public void AddEnemy(Transform enemy)
    {
        AllEnemies.Add(enemy);
    }

    public void RemoveEnemy(Transform enemy)
    {
        if (AllEnemies.Contains(enemy))
        {
            AllEnemies.Remove(enemy);
        }
        else
        {
            Debug.LogWarning("Enemy not found in the list.");
        }
    }

    public Transform FindClosestEnemy()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in AllEnemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    public void ShowEndScreen(bool didPlayerWin)
    {
        if (!didPlayerWin)
        {
            loseBackground.SetActive(true);
            messageText.gameObject.transform.localPosition = new Vector3(0, -66, 0);
        }
        else
            background.SetActive(true);
        messageText.text = "";
        StartCoroutine(RevealMessage(didPlayerWin));
    }

    private IEnumerator RevealMessage(bool didPlayerWin)
    {
        if (didPlayerWin)
        {
            foreach (var winMessage in winMessages)
            {
                yield return StartCoroutine(RevealText(winMessage));
            }
        }
        else
        {
            yield return StartCoroutine(RevealText(lossMessages[Random.Range(0, lossMessages.Count)]));
            //foreach (var lossMessage in lossMessages)
            //{
            //    yield return StartCoroutine(RevealText(lossMessage));
            //}
        }

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        SpawnButton(didPlayerWin);
    }

    private IEnumerator RevealText(string message)
    {
        messageText.text = "";

        foreach (char letter in message)
        {
            messageText.text += letter;
            yield return new WaitForSeconds(wordDelay);
        }

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    private void SpawnButton(bool didPlayerWin)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        endScreenButton.SetActive(true);
        Button button = endScreenButton.GetComponent<Button>();

        if (!didPlayerWin)
        {
            buttonText.text = "Try Again";
            button.onClick.AddListener(() => SceneManager.LoadScene("Arena"));
        }
        else
        {
            buttonText.text = "Quit";
            button.onClick.AddListener(() => Application.Quit());
        }
    }
}
