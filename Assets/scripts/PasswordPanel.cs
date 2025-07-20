using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;


public class PasswordPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField passwordInput;
    private Chest currentChest;
    public TextMeshProUGUI errorMessageText;
    private bool isActive = false;
    private MonoBehaviour playerMovementScript;
    public GameObject pauseButton;
    public TextMeshProUGUI optionalMessageText;
    public GameObject joystick; // Riferimento al joystick virtuale
    public GameObject healthBarUI;
    public GameObject shieldBarUI;
    public GameObject livesPanelUI;
    public GameObject AttackButton;
    public Volume blurVolume; // Riferimento al volume del blur



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

        Time.timeScale = 0f;    // Pausa il gioco quando il menu è attivo, ripristina la velocità del gioco quando il menu è nascosto
        blurVolume.enabled = true;
        joystick.SetActive(false); // Nasconde il joystick quando il menu è aperto
        // Nascondi barre
        healthBarUI.SetActive(false);
        shieldBarUI.SetActive(false);
        livesPanelUI.SetActive(false); // Nasconde il pannello delle vite quando il menu è aperto
        AttackButton.SetActive(false);

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

        string message = chest.GetOptionalMessage();
        if(!string.IsNullOrEmpty(message))
        {
            optionalMessageText.text = message;
            optionalMessageText.gameObject.SetActive(true);
        }
        else
        {
            optionalMessageText.gameObject.SetActive(false);
        }
    

    }

     public void OnConfirmButtonPressed()
    {
        string enteredPassword = passwordInput.text;

        if (currentChest != null)
        {

            if (currentChest.TryUnlock(enteredPassword))
            {
                ClosePanel();
            }
            else
            {
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

        Time.timeScale = 1f;          // Ripristina la velocità del gioco (se il gioco era messo in pausa)
        blurVolume.enabled = false;
        joystick.SetActive(true); // Mostra il joystick quando il menu è chiuso
        // Mostra barre
        healthBarUI.SetActive(true);
        shieldBarUI.SetActive(true);
        livesPanelUI.SetActive(true); // Mostra il pannello delle vite quando il menu è chiuso
        AttackButton.SetActive(true);

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