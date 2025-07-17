using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup transitionPanel;
    public float fadeDuration = 1f;

    public static SceneTransitionManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Opzionale
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSceneTransition(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        transitionPanel.gameObject.SetActive(true);

        // Fade-out (nero)
        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(5f);

        // Caricamento della scena
        yield return SceneManager.LoadSceneAsync(sceneName);

    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        while (timer <= fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            transitionPanel.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }
        transitionPanel.alpha = endAlpha;
    }
}