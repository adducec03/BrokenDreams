using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    public string BookID { get; private set; }

    [TextArea(2, 10)]
    public string[] pages;

    public string bookTitle = "Titolo del Libro";

    private BookPanel bookPanel;
    [SerializeField] private GameObject hintObject;


    void Start()
    {
        bookPanel = FindFirstObjectByType<BookPanel>();
        hintObject.SetActive(false);
        BookID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool CanInteract()
    {
        return !bookPanel.IsPanelActive(); // Evita doppia apertura
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        if (bookPanel != null)
        {
            bookPanel.OpenBook(this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintObject != null)
        {
            hintObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintObject != null)
        {
            hintObject.SetActive(false);
        }
    }
}
