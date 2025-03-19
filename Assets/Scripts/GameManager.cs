using UnityEngine;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.UI;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //  Singleton

    private int debugCounter = 0;

    private bool isGameActive = false;
    private bool isGameLevelActive = false;

    private bool isTutorialActive = false;

    private bool wasDiamondSlicedWhenBossAskedToStop = false;
    // [SerializeField] private TMP_Text textScore;
    private int score = 0;
    private int countBadDiamondsCut = 0;
    private int countGoodDiamondsCut = 0;

    private int countDiamondsCutWhenBossNotAllowed = 0;

    //Spawned good diamonds are spawned diamonds that have of the same 
    //color as the big diamond displayed before they were displayed
    private int countSpawnedGoodDiamonds = 0;
    private Color bigDiamondColor;
    [Header("Audio Clips")]
    public AudioClip successSound;
    public AudioClip failSound;
    private AudioSource audioSource;
    [Header("UI Elements")]
    public GameObject endGamePanel;
    public GameObject menuPanel;

    // public TextMeshProUGUI txtScore;
    // public TextMeshProUGUI txtGoodDiamondsCut;
    // public TextMeshProUGUI txtBadDiamondsCut;
    // public TextMeshProUGUI txtCountSpawnedGoodDiamonds;
    // public TextMeshProUGUI txtCountDiamondsCutWhenBossNotAllowed;
    public bool IsGameActive => isGameActive;
    public bool IsGameLevelActive => isGameLevelActive;
    public bool IsTutorialActive => isTutorialActive;
    public bool WasDiamondSlicedWhenBossAskedToStop => wasDiamondSlicedWhenBossAskedToStop;
    public int CountGoodDiamondsCut => countGoodDiamondsCut;
    public int CountBadDiamondsCut => countBadDiamondsCut;
    public int CountSpawnedGoodDiamonds => countSpawnedGoodDiamonds;
    public int CountDiamondsCutWhenBossNotAllowed => countDiamondsCutWhenBossNotAllowed;
    public Color BigDiamondColor => bigDiamondColor;

    public Button btnMainMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

   }
    

    private void Start()
    {
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource was missing - Created new one on GameManager!");
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;



    }


    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }
    public void SetGameLevelActive(bool active)
    {
        isGameLevelActive = active;
    }
    public void SetTutorialActive(bool active)
    {
        isTutorialActive = active;
    }

    public void SetWasDiamondSlicedWhenBossAskedToStop(bool val)
    {
        wasDiamondSlicedWhenBossAskedToStop = val;
    }

    public void AddScore(int points)
    {
        score += points;
        if (score < 0)
        {
            score = 0;
        }

        // if (score == 2)
        // {
        //     Time.timeScale = 0f;
        //     Time.fixedDeltaTime = 0f;
        // }

        Debug.Log("🎯 Score Updated: " + score);
        if (TutorialManager.Instance.TextScore != null)
        {
       TutorialManager.Instance.SetTextScore(score);
        }
       // textScore.text = $"Score: {score}";
    }

    // public void ResumeGame()
    // {
    //     Time.timeScale = 1f;
    //     Time.fixedDeltaTime = 0.02f * Time.timeScale;
    // }


    public void SetCountBadDiamondsCut(int val)
    {
        countBadDiamondsCut = val;
    }
    public void SetCountGoodDiamondsCut(int val)
    {
        countGoodDiamondsCut = val;
    }
    public void AddToCountSpawnedGoodDiamonds(int val)
    {
        countSpawnedGoodDiamonds += val;
    }
    public void AddToCountBadDiamondsCut(int points)
    {
        countBadDiamondsCut += points;
    }
    public void AddToCountGoodDiamondsCut(int points)
    {
        countGoodDiamondsCut += points;
    }
    public void AddToCountDiamondsCutWhenBossNotAllowed(int val)
    {
        countDiamondsCutWhenBossNotAllowed += val;
    }
    public int GetScore()
    {
        return score;
    }
    public void SetScore(int score)
    {
        this.score = score;
        if (TutorialManager.Instance.TextScore != null)
        {
       TutorialManager.Instance.SetTextScore(score);
        }

    }
    public void SetBigDiamondColor(Color color)
    {
        bigDiamondColor = color;
    }

    public void PlaySound(bool isSuccess)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is still missing on GameManager!");
            return;
        }

        if (isSuccess && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
            Debug.Log("Playing success sound...");
        }
        else if (!isSuccess && failSound != null)
        {
            audioSource.PlayOneShot(failSound);
            Debug.Log("Playing fail sound...");
        }
        else
        {
            Debug.LogWarning("No sound assigned in GameManager!");
        }
    }

    public void EndGame()
    {
        // isGameActive = false;
        // endGamePanel.SetActive(true);
        // Debug.Log("📌 Resetting table values...");
        // txtScore.text = "0";
        // txtGoodDiamondsCut.text = "0";
        // txtBadDiamondsCut.text = "0";
        // txtCountSpawnedGoodDiamonds.text = "0";
        // if (txtScore != null)
        //     txtScore.text = "Score: " + score;

        // if (txtGoodDiamondsCut != null)
        //     txtGoodDiamondsCut.text = "Good Diamonds cut: " + countGoodDiamondsCut;

        // if (txtBadDiamondsCut != null)
        //     txtBadDiamondsCut.text = "Bad Diamonds cut: " + countBadDiamondsCut;

        // int missedGoodDiamonds = countSpawnedGoodDiamonds - countGoodDiamondsCut;
        // txtCountSpawnedGoodDiamonds.text = $"Missed {missedGoodDiamonds} out of {countSpawnedGoodDiamonds} good diamonds";

        // txtCountDiamondsCutWhenBossNotAllowed.text =
        //     $"Diamonds cut when boss not allowed: {GameManager.Instance.CountDiamondsCutWhenBossNotAllowed}";

        // Debug.Log("Game Over!");
    }

    public bool ColorsMatch(Color a, Color b)
    {
        float tolerance = 0.05f;
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}

