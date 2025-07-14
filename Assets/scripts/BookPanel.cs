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
    public GameObject AttackButton; // Riferimento al bottone di attacco

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

        UpdateButtonsState();
        pageText.alpha = 0f;

        panel.SetActive(true);
        isActive = true;

        Time.timeScale = 0f;
        blurVolume.enabled = true;
        joystick.SetActive(false);
        healthBarUI.SetActive(false);
        shieldBarUI.SetActive(false);
        livesPanelUI.SetActive(false);
        AttackButton.SetActive(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (pauseButton != null)
            pauseButton.SetActive(false);

        StartCoroutine(FadeTextTransition(GetPageText(currentPage)));
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

        for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
        {
            pageText.alpha = Mathf.Lerp(1f, 0f, t / halfDuration);
            yield return null;
        }
        pageText.alpha = 0f;

        pageText.text = newText;
        UpdateButtonsState();

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
            StartCoroutine(FadeTextTransition(GetPageText(currentPage)));
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            StartCoroutine(FadeTextTransition(GetPageText(currentPage)));
        }
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        isActive = false;

        Time.timeScale = 1f;
        blurVolume.enabled = false;
        joystick.SetActive(true);
        healthBarUI.SetActive(true);
        shieldBarUI.SetActive(true);
        livesPanelUI.SetActive(true);
        AttackButton.SetActive(true);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    private string GetPageText(int pageIndex)
    {
        string rawText = currentBook.pages[pageIndex];

        if (pageIndex == 0 && !string.IsNullOrEmpty(SessionManager.currentUsername))
        {
            rawText = rawText.Replace("Giocatore", SessionManager.currentUsername);
        }

        return rawText;
    }
}
