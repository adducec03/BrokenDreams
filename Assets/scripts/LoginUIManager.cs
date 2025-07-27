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
    public TMP_InputField loginPassword;
    public TMP_InputField loginEmail;

    [Header("Register Fields")]
    public TMP_InputField registerUsername;
    public TMP_InputField registerPassword;
    public TMP_InputField registerEmail;

    [Header("Bottoni")]
    public UnityEngine.UI.Button RegisterButton;
    public UnityEngine.UI.Button LoginButton;


    [Header("Messaggi")]
    public TextMeshProUGUI messageText;

    private void Start()
    {
        RegisterButton.interactable = false;
        LoginButton.interactable = false;
        ShowMain();

        StartCoroutine(WaitForFirebaseReady());
    }


    private IEnumerator WaitForFirebaseReady()
    {
        // Step 1: aspetta che FirebaseManager.Instance sia inizializzato
        while (FirebaseManager.Instance == null)
        {
            messageText.text = "Inizializzazione Firebase...";
            yield return null;
        }

        // Step 2: aspetta che Firebase abbia completato l'inizializzazione
        while (!FirebaseManager.Instance.IsFirebaseReady)
        {
            messageText.text = "Inizializzazione Firebase...";
            yield return null;
        }

        // Step 3: tutto pronto
        RegisterButton.interactable = true;
        LoginButton.interactable = true;
        messageText.text = "";
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
        string email = registerEmail.text.Trim().ToLower();
        string password = registerPassword.text;
        string username = registerUsername.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            ShowMessage("Compila tutti i campi", Color.red);
            return;
        }

        FirebaseManager.Instance.Register(email, password, username, (success, message) =>
        {
            if (success)
            {
                ShowMessage("Registrazione effettuata con successo!", Color.green);
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                Debug.Log("In qualche modo entro qui");
                ShowMessage(message, Color.red);
            }
        });
    }

    public void ConfirmLogin()
    {
        string email = loginEmail.text.Trim().ToLower(); // oppure usa quello registrato
        string password = loginPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Inserisci email e password", Color.red);
            return;
        }

        FirebaseManager.Instance.Login(email, password, (success, message) =>
        {
            if (success)
            {
                ShowMessage("Login avvenuto con successo", Color.green);
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                ShowMessage(message, Color.red);
            }
        });
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
