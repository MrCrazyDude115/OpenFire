using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Resource for SFXManager: https://youtu.be/DU7cgVsU2rM?si=l8D2z5PyelZm2wtd

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] public AudioSource SFXObject;
    [SerializeField] public AudioSource musicObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {

    }

    // Resource for IEnumerator FadeIn/Out: https://funkyton.com/unity-audio-fade-in-out-function-c/

    public static IEnumerator FadeIn(AudioSource audioSource, float volume, float fadeTime)
    {
        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
    }

    public void PlaySFXClip(AudioResource audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(SFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.resource = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        //float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, 5);
    }

    public void PlayMusicClip(AudioResource audioClip, float volume, float fadeTime)
    {
        musicObject.resource = audioClip;
        StartCoroutine(FadeIn(musicObject, volume, fadeTime));
    }

    public void StopMusicClip(float fadeTime)
    {
        StartCoroutine(FadeOut(musicObject, fadeTime));
    }
}
