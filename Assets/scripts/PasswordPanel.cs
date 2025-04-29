using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField passwordInput;
    private Chest currentChest;

    void Start()
    {
        panel.SetActive(false);
    }

    public void OpenPasswordPrompt(Chest chest)
    {
        currentChest = chest;
        passwordInput.text = "";
        panel.SetActive(true);
        passwordInput.ActivateInputField();
    }

    public void OnConfirmButtonPressed()
    {
        if (currentChest != null)
        {
            string enteredPassword = passwordInput.text;
            currentChest.TryUnlock(enteredPassword);
        }
        panel.SetActive(false);
    }
}
