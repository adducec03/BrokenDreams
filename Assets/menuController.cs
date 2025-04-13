using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class menuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public Button menuButton; // Riferimento al pulsante del menu
    public Volume blurVolume; // Riferimento al volume del blur
    public GameObject joystick; // Riferimento al joystick virtuale
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
        if(Input.GetKeyDown(KeyCode.Tab))
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
    }

    public void ReturnToGame()
    {
        menuCanvas.SetActive(false);  // Nasconde il menu
        Time.timeScale = 1f;          // Ripristina la velocità del gioco (se il gioco era messo in pausa)
        blurVolume.enabled = false; // Disabilita il blur quando il menu è chiuso
        menuButton.gameObject.SetActive(true); // Mostra il pulsante del menu
        joystick.SetActive(true); // Mostra il joystick quando il menu è aperto
    }
}
