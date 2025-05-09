using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if !UNITY_WEBGL
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
#endif
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

#if !UNITY_WEBGL
    private DatabaseReference dbRef;
#endif
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
#if !UNITY_WEBGL
    FirebaseApp
      .CheckAndFixDependenciesAsync()
      .ContinueWithOnMainThread(task =>
    {
        if (task.Result == DependencyStatus.Available)
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        else
            Debug.LogError("❌ Firebase error: " + task.Result);
    });
#else
        // WebGL: אין SDK
#endif
    }


    public void ResetLoginState()
    {
        currentUsername = null;
    }

    public async void OnLogin()
    {
        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ Please fill in all fields");
            return;
        }

#if UNITY_WEBGL
    string storedPassword = await FirebaseWebGL.GetPasswordAsync(username);
    if (string.IsNullOrEmpty(storedPassword))
    {
        Debug.LogWarning("⚠️ Username does not exist or error occurred");
        return;
    }

    if (storedPassword == password)
    {
        currentUsername = username;
        Debug.Log($"✅ Login successful (WebGL): {username}");
        loginPanel?.SetActive(false);
        playButton?.gameObject.SetActive(true);
        exitButton?.gameObject.SetActive(true);
    }
    else
    {
        Debug.LogWarning("⚠️ Incorrect password (WebGL)");
    }
#else
        // קוד מקורי
#endif
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

    public async void OnRegister()
    {
        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrWhiteSpace(username))
        {
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

#if UNITY_WEBGL
    bool exists = await FirebaseWebGL.CheckUserExistsAsync(username);
    if (exists)
    {
        registerErrorText.text = "משתמש קיים";
        Invoke(nameof(ClearRegisterErrorText), 5f);
        return;
    }

    bool success = await FirebaseWebGL.RegisterUserAsync(username, password);
    if (success)
        Debug.Log("✅ User registered successfully (WebGL)");
    else
        Debug.LogError("❌ Registration error (WebGL)");
#else
        // קוד מקורי
#endif
    }


    public async void SaveFullScoreData(int score, int goodCut, int badCut, int spawnedGood, int bossCut)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.Log("⚠️ No user logged in – data not saved");
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

        await FirebaseWebGL.SaveScoreAsync(currentUsername, scoreData);
    }


    public string GetCurrentUsername() => currentUsername;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("DiamondQuest");
    }

    public async void OnForgotPassword()
    {
        string username = usernameInput?.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            forgotPasswordText.text = "הכנס משתמש";
            Invoke(nameof(ClearForgotPasswordText), 5f);
            return;
        }

#if UNITY_WEBGL
    string password = await FirebaseWebGL.GetPasswordAsync(username);
    if (!string.IsNullOrEmpty(password))
    {
        string reversedPassword = ReverseDigits(password);
        forgotPasswordText.text = reversedPassword;
    }
    else
    {
        forgotPasswordText.text = "משתמש לא קיים";
    }
    Invoke(nameof(ClearForgotPasswordText), 5f);
#else
        // קוד מקורי
#endif
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