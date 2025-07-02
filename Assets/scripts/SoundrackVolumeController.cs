using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundtrackVolumeController : MonoBehaviour
{
    private AudioSource musicSource;

    void Awake()
    {
        musicSource = GetComponent<AudioSource>();

        // Carica volume salvato
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicSource.volume = savedVolume;
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;

        // Salva volume
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return musicSource.volume;
    }
}
