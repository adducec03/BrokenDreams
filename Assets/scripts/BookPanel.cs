using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;

public class BookPanel : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI pageText;
    public Button nextPageButton;
    public Button prevPageButton;
    public Button closeButton;
    private Book currentBook;
    private int currentPage = 0;
    private bool isActive = false;
    private MonoBehaviour playerMovementScript;
    public GameObject pauseButton;
    public GameObject joystick; // Riferimento al joystick virtuale
    public GameObject healthBarUI;
    public GameObject shieldBarUI;
    public GameObject livesPanelUI;
    public Volume blurVolume; // Riferimento al volume del blur


    void Start()
    {
        panel.SetActive(false);
        playerMovementScript = FindFirstObjectByType<PlayerMovement>();

        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PrevPage);
        closeButton.onClick.AddListener(ClosePanel);
    }

    public bool IsPanelActive()
    {
        return isActive;
    }

    public void OpenBook(Book book)
    {
        currentBook = book;
        currentPage = 0;

        titleText.text = book.bookTitle;

        // Aggiorna subito lo stato dei bottoni
        UpdateButtonsState();

        // Imposta testo invisibile prima di fare la dissolvenza
        pageText.alpha = 0f;

        panel.SetActive(true);
        isActive = true;

        Time.timeScale = 0f;    // Pausa il gioco quando il menu è attivo, ripristina la velocità del gioco quando il menu è nascosto
        blurVolume.enabled = IsPanelActive();
        joystick.SetActive(false); // Nasconde il joystick quando il menu è aperto
        // Nascondi barre
        healthBarUI.SetActive(false);
        shieldBarUI.SetActive(false);
        livesPanelUI.SetActive(false); // Nasconde il pannello delle vite quando il menu è aperto

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (pauseButton != null)
            pauseButton.SetActive(false);

        // Avvia la dissolvenza per mostrare la prima pagina
        StartCoroutine(FadeTextTransition(currentBook.pages[currentPage]));
    }

    private void UpdateButtonsState()
    {
        prevPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = currentPage < currentBook.pages.Length - 1;
    }

   private IEnumerator FadeTextTransition(string newText)
{
    float duration = 1f;
    float halfDuration = duration / 2f;

    // Fade out
    for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
    {
        pageText.alpha = Mathf.Lerp(1f, 0f, t / halfDuration);
        yield return null;
    }
    pageText.alpha = 0f;

    pageText.text = newText;
    UpdateButtonsState();

    // Fade in
    for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
    {
        pageText.alpha = Mathf.Lerp(0f, 1f, t / halfDuration);
        yield return null;
    }
    pageText.alpha = 1f;
}

    public void NextPage()
    {
        if (currentPage < currentBook.pages.Length - 1)
        {
            currentPage++;
            StartCoroutine(FadeTextTransition(currentBook.pages[currentPage]));
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            StartCoroutine(FadeTextTransition(currentBook.pages[currentPage]));
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

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }
}
