using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;

    [Header("Audio")]
    public AudioSource musicSource;

    void Start()
    {
        // Carica valori salvati, oppure usa default
        float musicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1f;
        float sfxVolume = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : 1f;
        float brightness = PlayerPrefs.HasKey("Brightness") ? PlayerPrefs.GetFloat("Brightness") : 0f;

        // Imposta gli slider SENZA triggerare eventi
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.RemoveAllListeners();

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        brightnessSlider.value = brightness;

        // Applica i valori direttamente
        UpdateMusicVolume(musicVolume);
        UpdateSFXVolume(sfxVolume);
        UpdateBrightness(brightness);

        // Aggiungi i listener solo dopo aver impostato i valori
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
    }


    public void UpdateMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void UpdateSFXVolume(float value)
    {
        SoundEffectManager.SetVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void UpdateBrightness(float value)
    {
        RenderSettings.ambientLight = Color.white * value;
        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }
}