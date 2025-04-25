using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    public Image fillImage;

    public void SetShield(float current, float max)
    {
        float fillAmount = current / max;
        fillImage.fillAmount = Mathf.Clamp01(fillAmount);
    }
}