using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Lettura Foglio")]
    public GameObject textPanel;
    public TextMeshProUGUI textDisplay;

    [Header("Controlli UI")]
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
        // Nasconde tutti gli elementi UI (Anche se sono gia disattivati, fa un controllo ulteriore. Utile per permettere alla pergamena di essere aperta anche dalla scena e non solo dall'inventario)
        pauseMenuController.menuCanvas.SetActive(false);
        pauseMenuController.attackButtonUI.SetActive(false);
        pauseMenuController.menuButton.gameObject.SetActive(false);
        pauseMenuController.healthBarUI.SetActive(false);
        pauseMenuController.shieldBarUI.SetActive(false);
        pauseMenuController.livesPanelUI.SetActive(false);
        pauseMenuController.joystick.SetActive(false);
        Time.timeScale = 0f;

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
