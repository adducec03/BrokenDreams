using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Lettura Foglio")]
    public GameObject textPanel;
    public TextMeshProUGUI textDisplay;

    [Header("Controlli menu")]
    public menuController pauseMenuController;

    private bool wasMenuActive;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowText(string content)
    {
        if (pauseMenuController != null)
        {
            pauseMenuController.menuCanvas.SetActive(false);
        }

        textPanel.SetActive(true);
        textDisplay.text = content;
    }

    public void HideText()
    {
        textPanel.SetActive(false);

        if (pauseMenuController != null)
        {
            pauseMenuController.menuCanvas.SetActive(true);
        }
    }

    public void CloseReadable()
    {
        HideText();
    }
}
