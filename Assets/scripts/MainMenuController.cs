using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject playPanel;
    

    public void Start()
    {
        playPanel.SetActive(false);  //Assicurati che il pannello del tasto Play sia nascosto all'inizio
        settingsPanel.SetActive(false); // Assicurati che il pannello delle impostazioni sia nascosto all'inizio
        mainPanel.SetActive(true); // Assicurati che il pannello principale sia visibile all'inizio
    }

    public void PlayGame()
    {
        settingsPanel.SetActive(false); // Nasconde il pannello delle impostazioni quando il playPanel è aperto
        mainPanel.SetActive(false); // Nasconde il pannello principale quando il playPanel è aperto
        playPanel.SetActive(true);
        
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false); // Nasconde il pannello principale quando le impostazioni sono aperte
        playPanel.SetActive(false); // Nasconde il pannello del tasto play quando le impostazioni sono aperte
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void CloseSettings()
    {
        playPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true); // Mostra di nuovo il pannello principale quando le impostazioni sono chiuse
    }

    public void SelectLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }

    public void SelectLevelTwo()
    {
        SceneManager.LoadScene("LivelliInCostruzione");
    }

    public void SelectLevelThree()
    {
        SceneManager.LoadScene("LivelliInCostruzione");
    }

    public void SelectLevelFour()
    {
        SceneManager.LoadScene("LivelliInCostruzione");
    }
}