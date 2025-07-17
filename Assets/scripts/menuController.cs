using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class menuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public Button menuButton; // Riferimento al pulsante del menu
    public Button exitButton; // Riferimento al pulsante di uscita
    public Volume blurVolume; // Riferimento al volume del blur
    public GameObject joystick; // Riferimento al joystick virtuale
    public GameObject healthBarUI;
    public GameObject shieldBarUI;
    public GameObject livesPanelUI;
    public GameObject attackButtonUI; // Riferimento al bottone di attacco


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuCanvas.SetActive(false);
        blurVolume.enabled = false; // Assicurati che il blur sia disabilitato all'inizio
        joystick.SetActive(true); // Assicurati che il joystick sia attivo all'inizio
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }

    public void ToggleMenu()
    {
        // Inverte la visibilità del menu
        bool isMenuActive = !menuCanvas.activeSelf;
        menuCanvas.SetActive(isMenuActive);

        Time.timeScale = 0f;    // Pausa il gioco quando il menu è attivo, ripristina la velocità del gioco quando il menu è nascosto
        blurVolume.enabled = isMenuActive;

        // Disabilita il pulsante quando il menu è attivo, riabilitalo quando il menu è nascosto
        menuButton.gameObject.SetActive(!isMenuActive); // Nasconde il pulsante quando il menu è aperto
        joystick.SetActive(!isMenuActive); // Nasconde il joystick quando il menu è aperto
        attackButtonUI.SetActive(!isMenuActive); // Nascondi il bottone per l'attacco
        // Nascondi barre
        healthBarUI.SetActive(false);
        shieldBarUI.SetActive(false);
        livesPanelUI.SetActive(false); // Nasconde il pannello delle vite quando il menu è aperto



    }

    public void ReturnToGame()
    {
        menuCanvas.SetActive(false);  // Nasconde il menu
        Time.timeScale = 1f;          // Ripristina la velocità del gioco (se il gioco era messo in pausa)
        blurVolume.enabled = false; // Disabilita il blur quando il menu è chiuso
        menuButton.gameObject.SetActive(true); // Mostra il pulsante del menu
        joystick.SetActive(true); // Mostra il joystick quando il menu è chiuso
        attackButtonUI.SetActive(true); // Mostra il pulsante di attacco quando il menu è chiuso

        // Mostra barre
        healthBarUI.SetActive(true);
        shieldBarUI.SetActive(true);
        livesPanelUI.SetActive(true);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}
