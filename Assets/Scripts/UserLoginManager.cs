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
    public Button forgotPasswordButton;
    public TMP_Text forgotPasswordText;
    public TMP_Text registerErrorText;
    public Button loginButton;
    public Button registerButton;
    public GameObject loginPanel;
    public Button playButton;
    public Button exitButton;

    private DatabaseReference dbRef;
    private string currentUsername;
    public static UserLoginManager Instance => instance;


    public void InitUI(
        TMP_InputField usernameInput,
        TMP_InputField passwordInput,
        Button forgotPasswordButton,
        TMP_Text forgotPasswordText,
        Button loginButton,
        Button registerButton,
        GameObject loginPanel,
        Button playButton,
        Button exitButton,
        TMP_Text registerErrorText)
    {
        this.usernameInput = usernameInput;
        this.passwordInput = passwordInput;
        this.forgotPasswordButton = forgotPasswordButton;
        this.forgotPasswordText = forgotPasswordText;
        this.registerErrorText = registerErrorText;
        this.loginButton = loginButton;
        this.registerButton = registerButton;
        this.loginPanel = loginPanel;
        this.playButton = playButton;
        this.exitButton = exitButton;

        usernameInput?.onValueChanged.AddListener(value => {
            if (string.IsNullOrWhiteSpace(value))
            {
                passwordInput.text = string.Empty;
            }
        });

        passwordInput?.onValueChanged.AddListener(value => {
            if (string.IsNullOrWhiteSpace(value))
            {
                ClearForgotPasswordText();
            }
        });

        loginButton?.onClick.RemoveAllListeners();
        registerButton?.onClick.RemoveAllListeners();
        loginButton?.onClick.AddListener(OnLogin);
        registerButton?.onClick.AddListener(OnRegister);

        forgotPasswordButton?.onClick.RemoveAllListeners();
        forgotPasswordButton?.onClick.AddListener(OnForgotPassword);

        playButton?.onClick.RemoveAllListeners();
        playButton?.onClick.AddListener(TryPlay);

        if (!string.IsNullOrEmpty(currentUsername))
        {
            usernameInput.text = string.Empty;
            passwordInput.text = string.Empty;
            forgotPasswordText.text = "שכחתי סיסמה";
            loginPanel?.SetActive(false);
            playButton?.gameObject.SetActive(true);
            exitButton?.gameObject.SetActive(true);
        }
        else
        {
            usernameInput.text = string.Empty;
            passwordInput.text = string.Empty;
            forgotPasswordText.text = "שכחתי סיסמה";
            loginPanel?.SetActive(true);
            playButton?.gameObject.SetActive(false);
            exitButton?.gameObject.SetActive(false);
        }
    }
    private void Awake()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(gameObject); // שורד מעבר סצנות
    }
    else
    {
        Destroy(gameObject); // הורס עותקים נוספים
    }
}

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("❌ Firebase error: " + task.Result);
            }
        });
    }

    public void ResetLoginState()
    {
        currentUsername = null;
    }

    public void OnLogin()
    {
        if (dbRef == null)
        {
            Debug.LogError("❌ Firebase is not ready");
            return;
        }

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ Please fill in all fields");
            return;
        }

        dbRef.Child("users").Child(username).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.Result.Exists)
            {
                Debug.LogWarning("⚠️ Username does not exist");
            }
            else
            {
                string storedPassword = task.Result.Value.ToString();
                if (storedPassword == password)
                {
                    currentUsername = username;
                    Debug.Log($"✅ Login successful: {username}");

                    loginPanel?.SetActive(false);
                    playButton?.gameObject.SetActive(true);
                    exitButton?.gameObject.SetActive(true);

                    playButton?.onClick.RemoveAllListeners();
                    playButton?.onClick.AddListener(TryPlay);
                }
                else
                {
                    Debug.LogWarning("⚠️ Incorrect password");
                }
            }
        });
    }

    public void TryPlay()
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogWarning("⚠️ Cannot start game – not logged in");
            return;
        }

        LoadGameScene();
    }

    public void OnRegister()
    {
        if (dbRef == null)
        {
            Debug.LogError("❌ Firebase is not ready");
            return;
        }

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.Log("⛳ registerErrorText triggered – missing username");
            registerErrorText.text = "הכנס משתמש";
            registerErrorText.gameObject.SetActive(true);
            Invoke(nameof(ClearRegisterErrorText), 5f);
            return;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            registerErrorText.text = "הכנס סיסמה";
        registerErrorText.gameObject.SetActive(true);
            Invoke(nameof(ClearRegisterErrorText), 5f);
            return;
        }

        dbRef.Child("users").Child(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result.Exists)
            {
                registerErrorText.text = "משתמש קיים";
                Invoke(nameof(ClearRegisterErrorText), 5f);
            }
            else
            {
                dbRef.Child("users").Child(username).Child("password").SetValueAsync(password)
                .ContinueWithOnMainThread(saveTask =>
                {
                    if (saveTask.IsCompleted)
                        Debug.Log("✅ User registered successfully");
                    else
                        Debug.LogError("❌ Registration error: " + saveTask.Exception);
                });
            }
        });
    }

    public void SaveFullScoreData(int score, int goodCut, int badCut, int spawnedGood, int bossCut)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.Log("⚠️ No user logged in – data not saved");
            return;
        }

        if (dbRef == null)
        {
            Debug.LogError("❌ Firebase not initialized");
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
                    Debug.Log($"✅ Score saved successfully for {currentUsername}");
                else
                    Debug.LogError("❌ Score saving error: " + task.Exception);
            });
    }

    public string GetCurrentUsername() => currentUsername;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("DiamondQuest");
    }

    public void OnForgotPassword()
    {
        string username = usernameInput?.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            forgotPasswordText.text = "הכנס משתמש";
            Invoke(nameof(ClearForgotPasswordText), 5f);
            return;
        }

        dbRef.Child("users").Child(username).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string password = task.Result.Value.ToString();
                string reversedPassword = ReverseDigits(password);
                forgotPasswordText.text = reversedPassword;
                Invoke(nameof(ClearForgotPasswordText), 5f);
            }
            else
            {
                forgotPasswordText.text = "משתמש לא קיים";
                Invoke(nameof(ClearForgotPasswordText), 5f);
            }
        });
    }

    void ClearForgotPasswordText()
    {
        forgotPasswordText.text = "שכחתי סיסמה";
    }

    void ClearRegisterErrorText()
    {
        registerErrorText.text = "הרשמה";
        registerErrorText.gameObject.SetActive(true);
    }

    string ReverseDigits(string digits)
    {
        char[] arr = digits.ToCharArray();
        System.Array.Reverse(arr);
        return new string(arr);
    }
}