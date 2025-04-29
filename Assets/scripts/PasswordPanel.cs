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


    void Start()
    {
        panel.SetActive(false);
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
        panel.SetActive(true);
        passwordInput.ActivateInputField();
        isActive = true;
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
    }
}