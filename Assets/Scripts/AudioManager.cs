using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer masterMixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float volume01) // 0–1 slider
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume01, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("MusicVolume", dB);
    }

    public void SetSFXVolume(float volume01)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume01, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("SFXVolume", dB);
    }

    public void SetMasterVolume(float volume01)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume01, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }
}