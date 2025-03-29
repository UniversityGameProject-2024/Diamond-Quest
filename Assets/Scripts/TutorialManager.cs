using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Prefab Settings")]
    public GameObject[] diamondPrefabs;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public GameObject nextButton;
    public GameObject skipButton;
    public Button backToMenuButton;
    public GameObject endGamePanel;


    private int stepIndex = 0;
    private bool tutorialFinished = false;
    private Vector3 startPosition = new Vector3(0.5f, 0.5f, 10f);
    private GameObject[] spawnedDiamonds;
    private bool bigDiamondExits = false;
    private RandomSpawner spawner;
    private GameObject bigDiamondInstance;
    private string[] tutorialSteps = new string[]
    {
        "These are the diamonds you will see in the game.",
        "Remember and collect only this color.",
        "Next step the game begins, Let’s try playing!",
    };
    private Camera mainCamera;
    GameObject diamondTutorial;
    [SerializeField] private TMP_Text textScore;
    public TextMeshProUGUI txtScoreTable;
    public TextMeshProUGUI txtGoodDiamondsCut;
    public TextMeshProUGUI txtBadDiamondsCut;
    public TextMeshProUGUI txtCountSpawnedGoodDiamonds;
    public TextMeshProUGUI txtCountDiamondsCutWhenBossNotAllowed;

    public TMP_Text TextScore => textScore;



    private RandomSpawner randomSpawnerScript;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        if (diamondTutorial == null)
        {
            nextButton.GetComponent<Button>().interactable = true;
        }
    }

    public void Start()
    {

        // if (backToMenuButton != null)
        // {
        //     backToMenuButton.gameObject.SetActive(false);
        // }

        StartTutorial();
    }

    public void StartTutorial()
    {
        mainCamera = Camera.main;

        diamondTutorial = GameObject.Find("Diamond Tutorial");
        stepIndex = 0;
        randomSpawnerScript = GameObject.Find("RandomSpawner").GetComponent<RandomSpawner>();

        tutorialPanel.SetActive(true);
        GameManager.Instance.SetTutorialActive(true);
        GameManager.Instance.SetGameActive(false);
        nextButton.GetComponent<Button>().interactable = false;
    }
    private void SpawnBigDaimonds()
    {
        if (bigDiamondExits)
        {
            return;
        }
        bigDiamondExits = true;
        GameObject diamondPrefab = diamondPrefabs[UnityEngine.Random.Range(0, diamondPrefabs.Length)];
        Vector3 centerScreenPosition = new Vector3(0.5f, 0.5f, 10f);
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(centerScreenPosition);
        Quaternion fixedRotation = Quaternion.Euler(-30f, 1f, 1f);
        bigDiamondInstance = Instantiate(diamondPrefab, worldPosition, fixedRotation);
        bigDiamondInstance.transform.localScale = new Vector3(3f, 3f, 3f);
    }
    private void SpawnAllDiamonds()
    {
        if (spawnedDiamonds != null)
            return;
        spawnedDiamonds = new GameObject[diamondPrefabs.Length];
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
        float spacing = 4f;
        for (int i = 0; i < diamondPrefabs.Length; i++)
        {
            Vector3 spawnPos = screenCenter + new Vector3((i - diamondPrefabs.Length / 2) * spacing, 0, 0);
            Quaternion rotation = Quaternion.Euler(-30f, 0f, 0f);
            GameObject diamond = Instantiate(diamondPrefabs[i], spawnPos, rotation);
            diamond.transform.localScale = Vector3.one * 2f;
            DiamondLabel label = diamond.AddComponent<DiamondLabel>();
            spawnedDiamonds[i] = diamond;

            Debug.Log($"✅ Spawned Diamond: {diamond.name} at {spawnPos}");
        }
    }
    public void NextStep()
    {
        if (tutorialFinished) return;
        stepIndex++;

        Debug.Log($"StepIndex={stepIndex}");
        if (stepIndex > tutorialSteps.Length)
        {
            EndTutorial();
        }
        else
        {
            UpdateTutorialText();
        }
        if (stepIndex == 1)
        {
            SpawnAllDiamonds();
        }
        if (stepIndex == 2)
        {
            foreach (GameObject diamond in spawnedDiamonds)
            {
                Destroy(diamond);
            }
            SpawnBigDaimonds();
        }
        if (stepIndex == 3)
        {
            if (bigDiamondInstance != null)
            {
                Destroy(bigDiamondInstance);
                bigDiamondInstance = null;
            }
            Invoke("DeleteTutorialText", 5f);
            StartCoroutine(SpawnDiamonds());
        }
        if (stepIndex == 4)
        {
            GameObject[] smallDiamonds = GameObject.FindGameObjectsWithTag("Small Diamond");
            foreach (GameObject smallDiamond in smallDiamonds)
            {
                Destroy(smallDiamond);
            }
        }
    }
    private void DeleteTutorialText()
    {
        tutorialText.text = "";
    }
    private IEnumerator SpawnDiamonds()
    {
        int countSpawnedDiamonds = 0;
        while (GameManager.Instance.IsTutorialActive)
        {
            countSpawnedDiamonds++;
            randomSpawnerScript.SpawnRandomDiamondForTutorial();
            yield return new WaitForSeconds(1f);
        }
    }
    public void SkipTutorial()
    {
        EndTutorial();
    }
    private void UpdateTutorialText()
    {
        tutorialText.text = tutorialSteps[stepIndex - 1];
    }
    public void SetTextScore(int score)
    {
         Debug.Log("🎯 Score Updated: " + score);
        textScore.text = $"ניקוד: {score}";
    }
    public void EndTutorial()
    {
        if (GameManager.Instance == null)
        {
            return;
        }
        GameObject[] smallDiamonds = GameObject.FindGameObjectsWithTag("Small Diamond");
        foreach (GameObject smallDiamond in smallDiamonds)
        {
            Destroy(smallDiamond);
        }
        tutorialPanel.SetActive(false);
        GameManager.Instance.SetTutorialActive(false);
        GameManager.Instance.SetGameActive(true);
        GameManager.Instance.SetGameLevelActive(false);
        // New game will end in 300 seconds (30 seconds for testing)
        Invoke("EndGame", 300f);

        GameManager.Instance.SetScore(0);
        GameManager.Instance.SetCountGoodDiamondsCut(0);
        GameManager.Instance.SetCountBadDiamondsCut(0);

        GameObject gameOverPanel = GameObject.Find("GameOverPanel");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        BigDiamond.hasSpawned = false;
        StartCoroutine(randomSpawnerScript.ShowBigDiamond());
        if (backToMenuButton != null)
        {
            backToMenuButton.gameObject.SetActive(true);
        }

        // // -------------------------

        // Debug.Log("Finding BackToMenuButton");
        // Button btnMainMenu = GameObject.Find("BackToMenuButton")?.GetComponent<Button>();
        // Debug.Log($"btnMainMenu={btnMainMenu}");
        // if (btnMainMenu != null)
        // {
        //     Debug.Log("Adding listener to btnMainMenu");
        //     btnMainMenu.onClick.RemoveAllListeners();
        //     btnMainMenu.onClick.AddListener(GameManager.Instance.LoadMainMenu);
        // }



    }
    public void LoadMainMenu()
    {
        //menuPanel.SetActive(true);
       // SceneManager.LoadScene("MeinMenu");
         if (GameManager.Instance.IsGameActive) // ✅ הכפתור פועל רק כשהמשחק פעיל
        {
            Debug.Log(" Returning to Main Menu...");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("⚠️ Cannot return to menu before the tutorial is finished!");
        }

    }
    private void EndGame()
    {
        GameManager.Instance.SetGameActive(false);
        endGamePanel.SetActive(true);
        Debug.Log("📌 Resetting table values...");
        txtScoreTable.text = "0";
        txtGoodDiamondsCut.text = "0";
        txtBadDiamondsCut.text = "0";
        txtCountSpawnedGoodDiamonds.text = "0";
        if (txtScoreTable != null)
            txtScoreTable.text = "Score: " + GameManager.Instance.GetScore();

        if (txtGoodDiamondsCut != null)
            txtGoodDiamondsCut.text = "Good Diamonds cut: " + GameManager.Instance.CountGoodDiamondsCut;

        if (txtBadDiamondsCut != null)
            txtBadDiamondsCut.text = "Bad Diamonds cut: " + GameManager.Instance.CountBadDiamondsCut;

        int missedGoodDiamonds = GameManager.Instance.CountSpawnedGoodDiamonds - GameManager.Instance.CountGoodDiamondsCut;
        txtCountSpawnedGoodDiamonds.text = $"Missed {missedGoodDiamonds} out of {GameManager.Instance.CountSpawnedGoodDiamonds} good diamonds";

        txtCountDiamondsCutWhenBossNotAllowed.text =
            $"Diamonds cut when boss not allowed: {GameManager.Instance.CountDiamondsCutWhenBossNotAllowed}";

        Debug.Log("Game Over!");
      //  GameManager.Instance.EndGame();
    }    
}
