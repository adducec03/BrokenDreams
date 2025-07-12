using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


public class LoginUIManager : MonoBehaviour
{
    private Coroutine messageCoroutine;

    [Header("Pannelli")]
    public GameObject mainPanel;
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Login Fields")]
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;

    [Header("Register Fields")]
    public TMP_InputField registerUsername;
    public TMP_InputField registerPassword;

    [Header("Messaggi")]
    public TextMeshProUGUI messageText;

    private void Start()
    {
        ShowMain(); // Mostra il menu iniziale
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        messageText.text = "";
    }

    public void ShowLogin()
    {
        mainPanel.SetActive(false);
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        messageText.text = "";
    }

    public void ShowRegister()
    {
        mainPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        messageText.text = "";
    }

    public void ConfirmRegister()
    {
        List<UserData> users = UserDatabase.LoadUsers();
        string newUser = registerUsername.text.Trim();
        string newPass = registerPassword.text;

        if (string.IsNullOrEmpty(newUser) || string.IsNullOrEmpty(newPass))
        {
            ShowMessage("Inserisci username e password validi.",Color.red);
            return;
        }

        if (users.Exists(u => u.username == newUser))
        {
            ShowMessage("Utente gia' registrato!",Color.red);
            return;
        }

        users.Add(new UserData { username = newUser, password = newPass });
        UserDatabase.SaveUsers(users);

        SessionManager.SetCurrentUser(newUser);
        ShowMessage("Registrazione riuscita!", Color.green);
        SceneManager.LoadScene("MainMenu");
    }

    public void ConfirmLogin()
    {
        List<UserData> users = UserDatabase.LoadUsers();
        string user = loginUsername.text.Trim();
        string pass = loginPassword.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            ShowMessage("Inserisci username e password", Color.red);
            return;
        }

        if (users.Exists(u => u.username == user && u.password == pass))
        {
            SessionManager.SetCurrentUser(user);
            SceneManager.LoadScene("MainMenu");
        }
        else if (users.Exists(u => u.username == user))
        {
            ShowMessage("Password errata!", Color.red);
        }
        else
        {
            ShowMessage("Utente non trovato. registrati prima.", Color.red);
        }
    }

    private void ShowMessage(string text, Color color, float duration = 2f)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(ShowMessageRoutine(text, color, duration));
    }

    private IEnumerator ShowMessageRoutine(string text, Color color, float duration)
    {
        messageText.text = text;
        messageText.color = color;
        yield return new WaitForSeconds(duration);
        messageText.text = "";
    }

}
