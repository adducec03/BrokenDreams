using UnityEngine;
using TMPro;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float displayDuration = 2f;

    private Coroutine messageCoroutine;

    public void ShowMessage(string message)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        yield return new WaitForSeconds(displayDuration);
        messageText.text = "";
    }
}
