using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    [SerializeField] private string bookID;
    [TextArea(5, 10)]
    public string[] pages; // Testo per ogni pagina
    public string bookTitle = "Libro";

    private BookUIManager uiManager;

    private void Start()
    {
        uiManager = FindFirstObjectByType<BookUIManager>();
        if (string.IsNullOrEmpty(bookID))
        {
            bookID = GlobalHelper.GenerateUniqueID(gameObject);
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (uiManager != null)
        {
            uiManager.OpenBook(bookTitle, pages);
        }
    }
}
