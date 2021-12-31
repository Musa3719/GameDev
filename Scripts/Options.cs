using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public static Options instance;

    public float SoundVolume;
    public float MusicVolume;

    public Slider SoundSlider;
    public Slider MusicSlider;

    private void Awake()
    {
        instance = this;
        SoundVolume = PlayerPrefs.GetFloat("Sound", 0.5f);
        MusicVolume = PlayerPrefs.GetFloat("Music", 0.5f);
        SoundSlider.value = SoundVolume;
        MusicSlider.value = MusicVolume;
    }
    public void SoundVolumeChanged(float newValue)
    {
        SoundVolume = newValue;
        PlayerPrefs.SetFloat("Sound", newValue);
    }
    public void MusicVolumeChanged(float newValue)
    {
        MusicVolume = newValue;
        PlayerPrefs.SetFloat("Music", newValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
