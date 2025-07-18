using UnityEngine;
using TMPro; // Usa questa riga se usi TextMeshPro

public class RandomQuoteDisplay : MonoBehaviour
{
    [TextArea(2, 4)]
    public string[] quotes;

    public TextMeshProUGUI quoteText;

    void Start()
    {
        if (quotes.Length > 0 && quoteText != null)
        {
            int randomIndex = Random.Range(0, quotes.Length);
            quoteText.text = quotes[randomIndex];
        }
    }
}