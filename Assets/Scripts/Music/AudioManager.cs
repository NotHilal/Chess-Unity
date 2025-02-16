using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource BgMusic;
    private int CurrentlyPlaying = 0;
    public AudioClip[] Musics;
    private float musicVolume = 0.02f;
    public bool isPlaying = true;
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;
    private void Awake()
    {
        
    }
    private void Start()
    {
        BgMusic.clip = Musics[CurrentlyPlaying];
        BgMusic.Play();
        
    }
    private void Update()
    {
        
        if(isPlaying)
        {
            if(BgMusic.isPlaying==false)
            {
                NextMusic();
            }
        }
        BgMusic.volume = musicVolume;
    }

    public void NextMusic()
    {
        
        BgMusic.Stop(); 
        if (BgMusic.volume == 0)
        {
            BgMusic.volume = 0.02f;
        }
        CurrentlyPlaying++;
        if(CurrentlyPlaying == Musics.Length)
        {
            CurrentlyPlaying = 0;
        }
        BgMusic.clip = Musics[CurrentlyPlaying];
        BgMusic.Play();
    }

    public void PreviousMusic()
    {
        
        BgMusic.Stop();
        if (BgMusic.volume == 0)
        {
            BgMusic.volume = 0.02f;
        }
        CurrentlyPlaying--;
        if (CurrentlyPlaying == -1)
        {
            CurrentlyPlaying = Musics.Length-1;
        }
        BgMusic.clip = Musics[CurrentlyPlaying];
        BgMusic.Play();
    }
    public void StopMusic()
    {
        if(isPlaying==true)
        {
            BgMusic.Pause();
            isPlaying = false;
        }
        
        
    }
    public void PlayMusic()
    {
        if(isPlaying==false)
        {
            BgMusic.Play();
            isPlaying = true;
        }
        

    }
    

    public void UpdateVolume(float Volume)
    {
        musicVolume = Volume;
        slider1.value = Volume;
        slider2.value = Volume;
        slider3.value = Volume;
        slider4.value = Volume;
    }
}
