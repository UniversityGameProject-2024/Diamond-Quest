using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class UserLoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public GameObject loginPanel;

    public GameObject playButton;
    public GameObject exitButton;

    private DatabaseReference dbRef;
    private string currentUsername;

    void Start()
    {
        // 🔁 בדיקה אם יש משתמש שמור לכניסה אוטומטית
        string savedUser = PlayerPrefs.GetString("username", "");
        if (!string.IsNullOrEmpty(savedUser))
        {
            Debug.Log($"🔁 התחברות אוטומטית בתור {savedUser}");
            currentUsername = savedUser;

            loginPanel.SetActive(false);
            playButton.SetActive(true);
            exitButton.SetActive(true);
            return;
        }

        // בדיקת זמינות Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase מוכן");
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                registerButton.onClick.AddListener(OnRegister);

                // בהתחלה נסתיר כפתורים
                playButton.SetActive(false);
                exitButton.SetActive(false);
            }
            else
            {
                Debug.LogError("❌ Firebase לא מוכן: " + task.Result);
            }
        });
    }

    void OnRegister()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ נא למלא שם וסיסמה");
            return;
        }

        currentUsername = username;

        dbRef.Child("users").Child(username).Child("password").SetValueAsync(password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"✅ המשתמש '{username}' נרשם בהצלחה!");

                // 🔐 שמירה מקומית של המשתמש לכניסה אוטומטית
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.Save();

                // הסתרת הפאנל
                if (loginPanel != null && loginPanel.GetComponent<PanelFader>() != null)
                {
                    loginPanel.GetComponent<PanelFader>().FadeOutAndDisable();
                }
                else
                {
                    loginPanel.SetActive(false);
                }

                // הפעלת כפתורים
                playButton.SetActive(true);
                exitButton.SetActive(true);
            }
            else
            {
                Debug.LogError("❌ שגיאה בשמירת המשתמש: " + task.Exception);
            }
        });
    }

    public void SaveScore(int score)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogWarning("⚠️ אין משתמש מחובר – לא נשמר ניקוד");
            return;
        }

        dbRef.Child("users").Child(currentUsername).Child("score").SetValueAsync(score).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log($"✅ ניקוד {score} נשמר עבור {currentUsername}");
            else
                Debug.LogError("❌ שגיאה בשמירת ניקוד: " + task.Exception);
        });
    }
}
