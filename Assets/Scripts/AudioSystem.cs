using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSystem : MonoBehaviour
{
    public AudioSource moveSound;
    public AudioSource dealDeckSound;
    public AudioSource[] winSounds;
    [SerializeField] Toggle audioTog;
    public bool audioOn;

    void Start()
    {
        audioOn = audioTog.isOn;
    }

    
    public void toggleAudio()
    {
        audioOn = audioTog.isOn;
        if (audioOn)
        {
            changeVolume(1f);
        }
        else
        {
            changeVolume(0f);
        }
    }

    void changeVolume(float volume)
    {
        moveSound.volume = volume;
        dealDeckSound.volume = volume;
        foreach (var sound in winSounds)
        {
            sound.volume = volume;
        }
    }
}
