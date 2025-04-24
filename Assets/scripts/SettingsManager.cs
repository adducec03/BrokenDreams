using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;

    void Start()
    {
        // Carica i valori salvati (se presenti)
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);

        // Applica subito i valori
        UpdateMusicVolume(musicSlider.value);
        UpdateSFXVolume(sfxSlider.value);
        UpdateBrightness(brightnessSlider.value);
    }

    public void UpdateMusicVolume(float value)
    {
        // Qui dovresti collegare il mixer audio
        AudioListener.volume = value; // o gestire tramite AudioMixer
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void UpdateSFXVolume(float value)
    {
        // Stesso discorso, collegalo al mixer effetti
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void UpdateBrightness(float value)
    {
        // Puoi controllare la luminosità della scena
        // esempio semplice: cambiare colore dell'UI
        RenderSettings.ambientLight = Color.white * value;
        PlayerPrefs.SetFloat("Brightness", value);
    }
}