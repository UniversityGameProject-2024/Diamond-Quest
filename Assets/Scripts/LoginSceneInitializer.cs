using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginSceneInitializer : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text forgotPasswordText;
    public TMP_Text registerErrorText;
   // public TMP_Text errorMessageText;

    public Button loginButton;
    public Button registerButton;
    public GameObject loginPanel;
    public Button playButton;
    public Button exitButton;
    public Button forgotPasswordButton;

    void Start()
    {
        var manager = FindObjectOfType<UserLoginManager>();
        if (manager != null)
        {
            manager.ResetLoginState(); // ← מתאפס אוטומטית בכל כניסה לתפריט
            manager.InitUI(
                usernameInput,
                passwordInput,
                forgotPasswordButton,
                forgotPasswordText,
                loginButton,
                registerButton,
                loginPanel,
                playButton,
                exitButton,
                registerErrorText
//errorMessageText
            );
        }
    }
}
