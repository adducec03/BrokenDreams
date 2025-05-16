using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        float duration = 1f;          // durata totale della dissolvenza
        float halfDuration = duration / 2f;

        // Fade out del testo corrente
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            pageText.alpha = Mathf.Lerp(1f, 0f, t / halfDuration);
            yield return null;
        }
        pageText.alpha = 0f;

        // Cambia il testo con quello nuovo
        pageText.text = newText;

        // Aggiorna bottoni (prev/next) in base alla pagina attuale
        UpdateButtonsState();

        // Fade in del nuovo testo
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
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

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }
}
