using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{
    public static SoundPlay instance;
    public GameObject AudioPrefab;

    private void Awake()
    {
        instance = this;
    }
    
    public void PlayClip(AudioClip clip, Vector3 position, float vol=1, float pitch=1)
    {
        GameObject audio = Instantiate(AudioPrefab, position, Quaternion.identity);
        audio.GetComponent<AudioSource>().clip = clip;
        audio.GetComponent<AudioSource>().pitch = pitch;
        audio.GetComponent<AudioSource>().volume = Options.instance.SoundVolume * vol;
        audio.GetComponent<AudioSource>().Play();
        //Destroy(audio, 2); it handles itself because of pauses
    }

}
