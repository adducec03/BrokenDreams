using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;

    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);

            float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            audioSource.volume = savedVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string soundName)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public static void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }
    
    public static void PlayAtPosition(string soundName, Vector3 position, float minDistance = 1f, float maxDistance = 10f)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            GameObject tempGO = new GameObject("TempAudio_" + soundName);
            tempGO.transform.position = position;

            AudioSource tempAudioSource = tempGO.AddComponent<AudioSource>();
            tempAudioSource.clip = audioClip;
            tempAudioSource.spatialBlend = 1f; // 3D sound
            tempAudioSource.minDistance = minDistance;
            tempAudioSource.maxDistance = maxDistance;
            tempAudioSource.volume = audioSource.volume;
            tempAudioSource.Play();

            GameObject.Destroy(tempGO, audioClip.length);
        }
    }

}
