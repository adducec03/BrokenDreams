using UnityEngine;
using UnityEngine.UI;

public class HeartsManager : MonoBehaviour
{
    public Image[] hearts; // Array dei cuori
    public Sprite fullHeart; // Sprite del cuore pieno
    public Sprite emptyHeart; // Sprite del cuore vuoto


    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lives)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}