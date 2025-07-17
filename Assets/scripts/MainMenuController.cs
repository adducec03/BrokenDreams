using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject playPanel;
    public GameObject loadingPanel;

    public void Start()
    {
        playPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
        if (loadingPanel != null) loadingPanel.SetActive(false); // Nascondi all'avvio
    }

    public void PlayGame()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false);
        playPanel.SetActive(false);
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
        mainPanel.SetActive(true);
    }

    public void SelectLevelOne()
    {
        SceneTransitionManager.Instance.StartSceneTransition("Level1");
    }

    public void SelectLevelTwo()
    {
        StartCoroutine(LoadLevelAsync("LivelliInCostruzione"));
    }

    public void SelectLevelThree()
    {
        StartCoroutine(LoadLevelAsync("LivelliInCostruzione"));
    }

    public void SelectLevelFour()
    {
        StartCoroutine(LoadLevelAsync("LivelliInCostruzione"));
    }

    // 👇 Metodo di caricamento con schermata
    private IEnumerator LoadLevelAsync(string sceneName)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}