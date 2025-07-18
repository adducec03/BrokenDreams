using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;

    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;
    private static List<AudioSource> activeLoopSources = new List<AudioSource>();

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

        foreach (AudioSource loopSource in activeLoopSources)
        {
            if (loopSource != null)
                loopSource.volume = volume;
        }
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

    public static AudioSource PlayLoopAtPosition(string soundName, Vector3 position, float minDistance, float maxDistance)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip == null) return null;

        GameObject loopGO = new GameObject("LoopAudio_" + soundName);
        loopGO.transform.position = position;

        AudioSource loopSource = loopGO.AddComponent<AudioSource>();
        loopSource.clip = audioClip;
        loopSource.spatialBlend = 1f;
        loopSource.minDistance = minDistance;
        loopSource.maxDistance = maxDistance;
        loopSource.loop = true;
        loopSource.volume = audioSource.volume;


        loopSource.rolloffMode = AudioRolloffMode.Custom;
        AnimationCurve customCurve = new AnimationCurve(
            new Keyframe(0f, 1f, 0f, 0f),        // Piatta, senza pendenza iniziale
            new Keyframe(18f, 1f, 0f, 0f),   // Ancora piatta in ingresso, ma in uscita inizia a calare
            new Keyframe(33f, 0.3f, -0.015f, -0.015f), // Punto intermedio con discesa dolce
            new Keyframe(70f, 0f, 0f, 0f)    // Fine curva, tangente quasi piatta
        );
        loopSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurve);

        loopSource.Play();
        activeLoopSources.Add(loopSource);

        return loopSource;
    }

    public static AudioSource PlayLoopOnObject(string soundName, GameObject parent)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip == null) return null;

        GameObject loopGO = new GameObject("LoopAudio_" + soundName);
        loopGO.transform.SetParent(parent.transform);
        loopGO.transform.localPosition = Vector3.zero;

        AudioSource loopSource = loopGO.AddComponent<AudioSource>();
        loopSource.clip = audioClip;
        loopSource.spatialBlend = 0f; // 2D sound
        loopSource.loop = true;
        loopSource.volume = audioSource.volume;

        loopSource.Play();
        activeLoopSources.Add(loopSource);

        return loopSource;
    }

}
