using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelFadeIn : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;

    void Start()
    {
        if (fadeCanvasGroup != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false); // disattiva per evitare blocchi UI
    }
}