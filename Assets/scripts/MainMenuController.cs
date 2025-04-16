using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    

    public void Start()
    {
        settingsPanel.SetActive(false); // Assicurati che il pannello delle impostazioni sia nascosto all'inizio
        mainPanel.SetActive(true); // Assicurati che il pannello principale sia visibile all'inizio
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false); // Nasconde il pannello principale quando le impostazioni sono aperte
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true); // Mostra di nuovo il pannello principale quando le impostazioni sono chiuse
    }
}