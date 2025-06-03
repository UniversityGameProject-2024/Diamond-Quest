using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Prefab Settings")]
    public GameObject[] diamondPrefabs;

    [Header("Timer UI")]
    [SerializeField] private TMPro.TMP_Text timerText;
    public GameObject timerPanel;

    [Header("Boss Prefab")]
    public GameObject bossPrefab;            // Prefab with animation
    [Tooltip("Adjust the scale of the boss instance when spawned")]
    public Vector3 bossScale = Vector3.one;  // New: boss display size
    [Tooltip("Adjust Y offset of boss spawn position")]
    public float bossYOffset = 0f;           // Vertical offset for boss
    private GameObject bossInstance;        // Reference to spawned boss

    [Header("Image Settings")]
    [Tooltip("Drag the UI Image GameObject (panel) to show in step 4")]
    public GameObject imageObject;
    [SerializeField] private Sprite[] tutorialSprites; // Sprites matching each tutorial step

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public GameObject nextButton;
    public GameObject skipButton;
    public Button backToMenuButton;
    public GameObject endGamePanel;
    public GameObject playAgainButton; // ← גרור את הכפתור מהאינספקטור


    public TextMeshProUGUI txtScoreTable;
    public TextMeshProUGUI txtGoodDiamondsCut;
    public TextMeshProUGUI txtBadDiamondsCut;
    public TextMeshProUGUI txtCountSpawnedGoodDiamonds;
    public TextMeshProUGUI txtCountDiamondsCutWhenBossNotAllowed;
    [SerializeField] private TMP_Text textScore;
    public TMP_Text TextScore => textScore;

    private UserLoginManager loginManager;
    private RandomSpawner randomSpawnerScript;
    private Camera mainCamera;
    private GameObject diamondTutorial;
    private GameObject[] spawnedDiamonds;
    private GameObject bigDiamondInstance;
    private int stepIndex = 0;
    private bool tutorialFinished = false;
    private bool bigDiamondExits = false;

    [SerializeField] private float gameDuration = 300f;
    private float remainingTime;
    private bool timerRunning = false;


    private string[] tutorialSteps = new string[]
    {
        "היהלומים שיופיעו במהלך המשחק.",
        "בתחילת המשחק ובכל פעם שתצבור 5 נקודות יופיע על המסך יהלום בצבע חדש. היהלום יופיע למשך 3 שניות ואז יעלם.\nעליך לתפוס יהלומים רק בצבע שהופיע.\nלדוגמא: כאן בתמונה יש לתפוס רק את היהלומים הכחולים",
        "אם שכחת את צבע היהלום שצריך לתפוס, אחרי 5 טעויות רצופות תופיע על המסך תזכורת.",
        "הבוס שלך, מר דין, יגיד לך מתי להפסיק לאסוף יהלומים. כאשר הוא ירים את ידו - עצור וכאשר הוא יוריד אותה - המשך",
        "אל תשכח שבמכרות המסוכנים יש הסחות דעת. אל תיתן להם להסיט אותך מהמטרה",
        "בשלב הבא המשחק מתחיל, בהצלחה!"
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(ExitGameEarly);

        loginManager = FindObjectOfType<UserLoginManager>();
        if (loginManager == null)
            Debug.LogError("❌ לא נמצא UserLoginManager – שמירת ניקוד תיכשל!");

        if (timerText != null)
        {
            timerPanel.SetActive(false);
            timerText.gameObject.SetActive(false);
        }

        StartTutorial();
    }

    private void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                timerRunning = false;
            }
            if (timerText != null)
                timerText.text = FormatTime(remainingTime);
        }

        // הקוד המקורי שלך ל־nextButton…
        if (diamondTutorial == null)
            nextButton.GetComponent<Button>().interactable = true;
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

    private void SpawnAllDiamonds()
    {
        if (spawnedDiamonds != null) return;

        spawnedDiamonds = new GameObject[diamondPrefabs.Length];
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
        float spacing = 4f;

        for (int i = 0; i < diamondPrefabs.Length; i++)
        {
            Vector3 spawnPos = screenCenter + new Vector3((i - diamondPrefabs.Length / 2) * spacing, 0, 0);
            GameObject diamond = Instantiate(diamondPrefabs[i], spawnPos, Quaternion.Euler(-30f, 0f, 0f));
            diamond.transform.localScale = Vector3.one * 2f;
            diamond.AddComponent<DiamondLabel>();
            spawnedDiamonds[i] = diamond;
        }
    }

    private void SpawnBigDaimonds()
    {
        if (bigDiamondExits) return;
        bigDiamondExits = true;

        GameObject diamondPrefab = diamondPrefabs[Random.Range(0, diamondPrefabs.Length)];
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        bigDiamondInstance = Instantiate(diamondPrefab, worldPosition, Quaternion.Euler(-30f, 1f, 1f));
        bigDiamondInstance.transform.localScale = new Vector3(3f, 3f, 3f);
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("⚠️ Boss Prefab לא הוגדר ב־Inspector");
            return;
        }
        Vector3 spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        spawnPos.y += bossYOffset; 
        bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        bossInstance.transform.localScale = bossScale;
    }


    public void NextStep()
    {
        if (tutorialFinished) return;
        stepIndex++;

        if (stepIndex > tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        UpdateTutorialText();

        if (stepIndex == 1)
            SpawnAllDiamonds();

        Image img = imageObject.GetComponent<Image>();

        if (stepIndex == 2)
        {
            foreach (GameObject diamond in spawnedDiamonds)
                Destroy(diamond);

            if (img != null && tutorialSprites != null && tutorialSprites.Length > 0)
                img.sprite = tutorialSprites[0];
            imageObject.SetActive(true);
        }

        if (stepIndex == 3)
        {
            if (img != null && tutorialSprites != null && tutorialSprites.Length > 0)
                img.sprite = tutorialSprites[1];
        }

        if (stepIndex == 4)
        {
            if (imageObject != null)
                imageObject.SetActive(false);
            SpawnBoss();
        }

        if (stepIndex == 5)
        {
            if (bossInstance != null)
                Destroy(bossInstance);
            if (img != null && tutorialSprites != null && tutorialSprites.Length > 0)
                img.sprite = tutorialSprites[2];
            imageObject.SetActive(true);
        }

        if (stepIndex == 6)
        {
            if (skipButton != null)
                skipButton.SetActive(false);
            TMP_Text btnText = nextButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = "התתחל משחק";

            if (imageObject != null)
                imageObject.SetActive(false);
            Invoke("DeleteTutorialText", 5f);
            StartCoroutine(SpawnDiamonds());
        }

        if (stepIndex == 7)
        {
            foreach (GameObject d in GameObject.FindGameObjectsWithTag("Small Diamond"))
                Destroy(d);
        }
    }

    private IEnumerator SpawnDiamonds()
    {
        while (GameManager.Instance.IsTutorialActive)
        {
            randomSpawnerScript.SpawnRandomDiamondForTutorial();
            yield return new WaitForSeconds(1f);
        }
    }

    private void DeleteTutorialText()
    {
        tutorialText.text = "";
    }

    public void SkipTutorial()
    {
        EndTutorial();
    }

    private void UpdateTutorialText()
    {
        tutorialText.text = tutorialSteps[stepIndex - 1];
    }

    //private void UpdateTutorialImage()
    //{
    //    tutorialText.text = tutorialSteps[stepIndex - 1];
    //}
    public float ElapsedTime
    {
        get
        {
            // gameDuration - remainingTime = הזמן שעבר מאז תחילת הספירה
            return Mathf.Clamp(gameDuration - remainingTime, 0f, gameDuration);
        }
    }
    public void SetTextScore(int score)
    {
        textScore.text = $"Score: {score}";
    }

    public void EndTutorial()
    {
        if (GameManager.Instance == null) return;

        foreach (GameObject d in GameObject.FindGameObjectsWithTag("Small Diamond"))
            Destroy(d);

        tutorialPanel.SetActive(false);
        GameManager.Instance.SetTutorialActive(false);
        GameManager.Instance.SetGameActive(true);
        GameManager.Instance.SetGameLevelActive(false);

        // להפעיל טיימר
        remainingTime = gameDuration;
        if (timerText != null)
        {
            timerPanel.SetActive(true);
            timerText.gameObject.SetActive(true);
            timerText.text = FormatTime(remainingTime);
        }
        timerRunning = true;
        Invoke("EndGame", gameDuration);

        GameManager.Instance.SetScore(0);
        GameManager.Instance.SetCountGoodDiamondsCut(0);
        GameManager.Instance.SetCountBadDiamondsCut(0);

        GameObject gameOverPanel = GameObject.Find("GameOverPanel");
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        BigDiamond.hasSpawned = false;
        StartCoroutine(randomSpawnerScript.ShowBigDiamond());

        if (backToMenuButton != null)
            backToMenuButton.gameObject.SetActive(true);

        if (PauseMenuController.Instance != null)
        {
            PauseMenuController.Instance.EnablePauseButton();
        }
    }

    public void LoadMainMenu()
    {
      //  if (GameManager.Instance.IsGameActive)
            SceneManager.LoadScene("MainMenu");
      //  else
         //   Debug.Log("⚠️ Cannot return to menu before the tutorial is finished!");
    }
   
    private void EndGame()
    {
        
        backToMenuButton.onClick.RemoveAllListeners(); // מסיר את כל הפעולות הקודמות
        backToMenuButton.onClick.AddListener(LoadMainMenu); // מוסיף פעולה חדשה

        GameManager.Instance.SetGameActive(false);
        endGamePanel.SetActive(true);

        txtScoreTable.text = "Score: " + GameManager.Instance.GetScore();
        txtGoodDiamondsCut.text = "Good Diamonds cut: " + GameManager.Instance.CountGoodDiamondsCut;
        txtBadDiamondsCut.text = "Bad Diamonds cut: " + GameManager.Instance.CountBadDiamondsCut;

        int missed = GameManager.Instance.CountSpawnedGoodDiamonds - GameManager.Instance.CountGoodDiamondsCut;
        txtCountSpawnedGoodDiamonds.text = $"Missed {missed} out of {GameManager.Instance.CountSpawnedGoodDiamonds} good diamonds";

        txtCountDiamondsCutWhenBossNotAllowed.text =
            $"Diamonds cut when boss not allowed: {GameManager.Instance.CountDiamondsCutWhenBossNotAllowed}";

        if (loginManager != null)
        {
            loginManager.SaveFullScoreData(
                GameManager.Instance.GetScore(),
                GameManager.Instance.CountGoodDiamondsCut,
                GameManager.Instance.CountBadDiamondsCut,
                GameManager.Instance.CountSpawnedGoodDiamonds,
                GameManager.Instance.CountDiamondsCutWhenBossNotAllowed
            );
        }
        else
        {
            Debug.LogWarning("⚠️ לא נשמרו תוצאות – loginManager לא אותחל.");
        }

        Debug.Log("Game Over!");
    }
    public void ExitGameEarly()
{
    Debug.Log("🚪 שחקן יצא מהמשחק מוקדם");

    // עצור את המשחק
    if (backToMenuButton != null && backToMenuButton.gameObject.activeSelf)
         backToMenuButton.gameObject.SetActive(false);



    // הצג את פאנל הסיום (הטבלה)
    if (endGamePanel != null)
        endGamePanel.SetActive(true);

    // עדכון הטקסטים בטבלה
    txtScoreTable.text = "Score: " + GameManager.Instance.GetScore();
    txtGoodDiamondsCut.text = "Good Diamonds cut: " + GameManager.Instance.CountGoodDiamondsCut;
    txtBadDiamondsCut.text = "Bad Diamonds cut: " + GameManager.Instance.CountBadDiamondsCut;

    int missedGoodDiamonds = GameManager.Instance.CountSpawnedGoodDiamonds - GameManager.Instance.CountGoodDiamondsCut;
    txtCountSpawnedGoodDiamonds.text = $"Missed {missedGoodDiamonds} out of {GameManager.Instance.CountSpawnedGoodDiamonds} good diamonds";

    txtCountDiamondsCutWhenBossNotAllowed.text =
        $"Diamonds cut when boss not allowed: {GameManager.Instance.CountDiamondsCutWhenBossNotAllowed}";

    // שמירת ניקוד בפיירבייס
    var loginManager = FindObjectOfType<UserLoginManager>();
    if (loginManager != null)
    {
        loginManager.SaveFullScoreData(
            GameManager.Instance.GetScore(),
            GameManager.Instance.CountGoodDiamondsCut,
            GameManager.Instance.CountBadDiamondsCut,
            GameManager.Instance.CountSpawnedGoodDiamonds,
            GameManager.Instance.CountDiamondsCutWhenBossNotAllowed
        );
    }
    else
    {
        Debug.LogWarning("⚠️ לא נמצא UserLoginManager – לא נשמר ניקוד");
    }
    if (playAgainButton != null)
        playAgainButton.SetActive(false);


    // מעבר לתפריט הראשי לאחר 2 שניות
    Invoke("LoadMainMenu", 5f);
}
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void ResumeTimer()
    {
        timerRunning = true;
    }

}
