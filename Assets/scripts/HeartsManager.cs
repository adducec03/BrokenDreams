using UnityEngine;
using UnityEngine.UI;

public class HeartsManager : MonoBehaviour
{
    public Image[] hearts; // Array dei cuori
    public Sprite fullHeart; // Sprite del cuore pieno
    public Sprite emptyHeart; // Sprite del cuore vuoto


    public void UpdateHearts(int lives)
    {
       int half = hearts.Length / 2;

    // Aggiorna i cuori della Game View
        for (int i = 0; i < half; i++)
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

        // Aggiorna i cuori del Menu
        for (int i = 0; i < half; i++)
        {
            if (i < lives)
            {
                hearts[i + half].sprite = fullHeart;  // ATTENZIONE: qui usi i+half
            }
            else
            {
                hearts[i + half].sprite = emptyHeart; // ATTENZIONE: qui usi i+half
            }
        }
    }
}