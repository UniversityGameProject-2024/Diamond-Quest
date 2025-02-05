using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Prefab Settings")]
    public GameObject[] diamondPrefabs; //  מערך של יהלומים לבחירה

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public GameObject nextButton;
    public GameObject skipButton;
    public Button backToMenuButton; 


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

        if (backToMenuButton != null)
        {
            backToMenuButton.gameObject.SetActive(false);
        }

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
     //   UpdateTutorialText();
         nextButton.GetComponent<Button>().interactable = false;
        
    }

    private void SpawnBigDaimonds(){
        if (bigDiamondExits) return; // ✅ מונע יצירה כפולה
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
        if (spawnedDiamonds != null) return; // ✅ מונע יצירה כפולה

        spawnedDiamonds = new GameObject[diamondPrefabs.Length];

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("❌ No Main Camera found!");
            return;
        }
    

        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f)); // ✅ מוודא שהיהלומים יהיו **בטווח הראייה של המצלמה**
        float spacing =4f; // ✅ ריווח בין היהלומים

        for (int i = 0; i < diamondPrefabs.Length; i++)
        {
            Vector3 spawnPos = screenCenter + new Vector3((i - diamondPrefabs.Length / 2) * spacing, 0, 0); // ✅ מסדר את היהלומים **באמצע המסך**
            Quaternion rotation = Quaternion.Euler(-30f, 0f, 0f); // ✅ מוסיף הטייה של -30° בציר X

            GameObject diamond = Instantiate(diamondPrefabs[i], spawnPos, rotation);
            diamond.transform.localScale = Vector3.one * 2f; // ✅ משנה את הגודל

             // ✅ הוספת כיתוב לכל יהלום
            DiamondLabel label = diamond.AddComponent<DiamondLabel>();
        //  label.diamondColorName = diamond.name.Replace("_", " "); // שם היהלום

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

        if(stepIndex == 1)
        {
           SpawnAllDiamonds(); //  מציג את כל היהלומים באוויר בשלב 2
            
        }
        if(stepIndex == 2)
        {
            foreach (GameObject diamond in spawnedDiamonds)
            {
                Destroy(diamond); 
            }
            SpawnBigDaimonds();  
            
        }
        if(stepIndex == 3)
        {
            if (bigDiamondInstance != null)
            {
               Destroy(bigDiamondInstance); // ✅ הורס את היהלום הגדול כשהמשתמש מתקדם משלב 2 לשלב 3
               bigDiamondInstance = null;
            }
            Invoke("DeleteTutorialText", 5f);
            StartCoroutine(SpawnDiamonds());
        }
        if(stepIndex == 4)
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
        while(GameManager.Instance.IsTutorialActive)
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
        tutorialText.text = tutorialSteps[stepIndex-1];
         
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

    }

    private void EndGame()
    {
        GameManager.Instance.EndGame();
    }
}
