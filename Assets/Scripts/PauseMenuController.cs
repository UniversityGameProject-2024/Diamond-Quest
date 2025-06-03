// PauseMenuController.cs

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("הפאנל שיוצג בעת עצירת המשחק")]
    public GameObject pauseMenuUI;

    [Tooltip("כפתור התפריט הראשי שבלחיצה עליו עוצרים את המשחק")]
    public Button mainMenuButton;

    [Tooltip("כפתור 'המשך' שיופעל מתוך PauseMenuPanel")]
    public Button continueButton;

    [Tooltip("כפתור 'יציאה' שיופעל מתוך PauseMenuPanel")]
    //public Button exitButton;

    // מאפשר גישה סטטית ל־PauseMenuController
    public static PauseMenuController Instance;

    private bool isPaused = false;

    private void Awake()
    {
        // וודא שיש רק מופע יחיד של הבקר
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // פאנל ההפסקה תמיד מוסתר בתחילת המשחק
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // הכפתור הראשי (mainMenuButton) מכובה בתחילת המשחק
        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        // אם בכל זאת הכפתור פעיל ברגע הפעלה, מוסיפים את ה־Listener
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        if (continueButton != null)
            continueButton.onClick.AddListener(ResumeGame);

        //if (exitButton != null)
        //    exitButton.onClick.AddListener(ExitToMainMenu);
    }

    private void OnMainMenuButtonClicked()
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        isPaused = true;

        // עצירת הזמן (כולל טיפוסי Update / FixedUpdate)
        Time.timeScale = 0f;

        // הצגת פאנל ההפסקה
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // עצירת הטיימר (אם יש TutorialManager)
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.PauseTimer();
    }

    public void ResumeGame()
    {
        isPaused = false;

        // הסתרת פאנל ההפסקה
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // החזרת הזמן לקדמותו
        Time.timeScale = 1f;

        // המשכת הטיימר
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.ResumeTimer();
    }

    //private void ExitToMainMenu()
    //{
    //    // בודקים שאם ירדו מהפסקה, הקצב יחזור לנורמה
    //    Time.timeScale = 1f;
    //    SceneManager.LoadScene("MainMenu");
    //}

    /// <summary>
    /// קוראים לפונקציה הזו כדי להפעיל את הכפתור Pause/Game Menu
    /// (למשל, אחרי שסיימנו את ההדרכה).
    /// </summary>
    public void EnablePauseButton()
    {
        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(true);
    }
}
