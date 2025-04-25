using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;  // La tua Global Light nella scena
    [SerializeField] private Slider brightnessSlider;

    private void Start()
    {
        // Imposta il valore iniziale dello slider a 0
        brightnessSlider.value = 0f;

        // Imposta la luminosità minima
        globalLight.intensity = 0f;

        // Aggiunge il listener all'evento dello slider
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
    }

    private void UpdateBrightness(float value)
    {
        // Limita l'intervallo tra 0 e 0.5
        globalLight.intensity = Mathf.Clamp(value, 0f, 0.5f);
    }
}