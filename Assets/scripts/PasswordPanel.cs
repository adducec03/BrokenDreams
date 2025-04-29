using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField passwordInput;
    private Chest currentChest;
    public TextMeshProUGUI errorMessageText;
    private bool isActive = false;
    private MonoBehaviour playerMovementScript;
    public GameObject pauseButton;


    void Start()
    {
        panel.SetActive(false);
        playerMovementScript = FindFirstObjectByType<PlayerMovement>();
    }

    public bool IsPanelActive()
    {
        return isActive;
    }


    public void OpenPasswordPrompt(Chest chest)
    {
        currentChest = chest;
        passwordInput.text = "";
        errorMessageText.text = "";

        var placeholder = passwordInput.placeholder as TextMeshProUGUI;
        if (placeholder != null)
        {
            placeholder.text = chest.passwordPlaceholder;
        }

        panel.SetActive(true);
        passwordInput.ActivateInputField();
        isActive = true;
        
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
        
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
    

    }

     public void OnConfirmButtonPressed()
    {
        string enteredPassword = passwordInput.text;

        if (currentChest != null)
        {
            Debug.Log($"Password inserita: {enteredPassword}");

            if (currentChest.TryUnlock(enteredPassword))
            {
                ClosePanel();
            }
            else
            {
                Debug.Log("Password Errata");
                errorMessageText.text = "Password Errata";
                passwordInput.ActivateInputField();
                passwordInput.Select();
            }
        }
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        isActive = false;
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

    }
}