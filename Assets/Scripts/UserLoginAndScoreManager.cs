using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UserLoginManager : MonoBehaviour
{
    private static UserLoginManager instance;

    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text forgotPasswordText; 
    public Button loginButton;
    public Button registerButton;
    public GameObject loginPanel;
    public GameObject playButton;
    public GameObject exitButton;


    private DatabaseReference dbRef;
    private string currentUsername;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "MainMenu")
        {
            if (!string.IsNullOrEmpty(currentUsername))
            {
                loginPanel?.SetActive(false);
                playButton?.SetActive(true);
                exitButton?.SetActive(true);
            }
            else
            {
                loginPanel?.SetActive(true);
                playButton?.SetActive(false);
                exitButton?.SetActive(false);
            }

            loginButton?.onClick.RemoveAllListeners();
            registerButton?.onClick.RemoveAllListeners();
            loginButton?.onClick.AddListener(OnLogin);
            registerButton?.onClick.AddListener(OnRegister);
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("❌ Firebase שגיאה: " + task.Result);
            }
        });
    }

    public void OnLogin()
    {
        if (dbRef == null)
        {
            Debug.LogError("❌ Firebase לא מוכן");
            return;
        }

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ נא למלא את כל השדות");
            return;
        }

        dbRef.Child("users").Child(username).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.Result.Exists)
            {
                Debug.LogWarning("⚠️ שם משתמש לא קיים");
            }
            else
            {
                string storedPassword = task.Result.Value.ToString();
                if (storedPassword == password)
                {
                    currentUsername = username;
                    Debug.Log($"✅ התחברות הצליחה: {username}");

                    loginPanel?.SetActive(false);
                    playButton?.SetActive(true);
                    exitButton?.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("⚠️ סיסמה שגויה");
                }
            }
        });
    }

    public void OnRegister()
    {
        if (dbRef == null)
        {
            Debug.LogError("❌ Firebase לא מוכן");
            return;
        }

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ נא למלא שם וסיסמה");
            return;
        }

        dbRef.Child("users").Child(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result.Exists)
            {
                Debug.LogWarning("⚠️ שם המשתמש כבר קיים");
            }
            else
            {
                dbRef.Child("users").Child(username).Child("password").SetValueAsync(password)
                .ContinueWithOnMainThread(saveTask =>
                {
                    if (saveTask.IsCompleted)
                        Debug.Log("✅ משתמש נרשם בהצלחה");
                    else
                        Debug.LogError("❌ שגיאה ברישום: " + saveTask.Exception);
                });
            }
        });
    }

    public void SaveFullScoreData(int score, int goodCut, int badCut, int spawnedGood, int bossCut)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.Log("⚠️ אין משתמש מחובר – לא נשמרו נתונים");
            return;
        }

        if (dbRef == null)
        {
            Debug.LogError("❌ Firebase לא מאותחל");
            return;
        }

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var scoreData = new Dictionary<string, object>
        {
            { "score", score },
            { "goodDiamondsCut", goodCut },
            { "badDiamondsCut", badCut },
            { "spawnedGoodDiamonds", spawnedGood },
            { "cutWhenBossNotAllowed", bossCut },
            { "timestamp", timestamp }
        };

        dbRef.Child("users").Child(currentUsername).Child("gameResults").Push().SetValueAsync(scoreData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log($"✅ ניקוד נשמר בהצלחה עבור {currentUsername}");
                else
                    Debug.LogError("❌ שגיאה בשמירת הניקוד: " + task.Exception);
            });
    }

    public string GetCurrentUsername() => currentUsername;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("DiamondQuest"); // שנה לשם הסצנה שלך
    }

public void OnForgotPassword()
{
    string username = usernameInput?.text.Trim();

    if (string.IsNullOrEmpty(username))
    {
        forgotPasswordText.text = "Please enter a username";
        return;
    }

    dbRef.Child("users").Child(username).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsCompleted && task.Result.Exists)
        {
            string password = task.Result.Value.ToString();

            // הפוך את המספר
            string reversedPassword = ReverseDigits(password);

            // הצג אותו
            forgotPasswordText.text = reversedPassword;
        }
        else
        {
            forgotPasswordText.text = "Username not found";
        }
    });
}

string ReverseDigits(string digits)
{
    char[] arr = digits.ToCharArray();
    System.Array.Reverse(arr);
    return new string(arr);
}


}
