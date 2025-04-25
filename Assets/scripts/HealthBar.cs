using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(float current, float max)
    {
        float fillAmount = current / max;
        fillImage.fillAmount = Mathf.Clamp01(fillAmount);
    }
}