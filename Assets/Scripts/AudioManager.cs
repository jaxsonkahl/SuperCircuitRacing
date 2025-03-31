using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] music, sfx;
    public AudioSource MusicSource, SfxSource;

     private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

   private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Menu"){
            PlayMusic("Menu");
            MusicSource.loop = true;
        }
       
    }

    public void PlayMusic(string name){
        Sound s = Array.Find(music, x => x.name == name);
        if(s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
        }else{
            MusicSource.clip = s.clip;
            MusicSource.Play();
            MusicSource.loop = true;
            MusicSource.volume = 60f;
            Debug.Log("Playing music: " + name);
            
        }
    }

    public void PlaySfx(string name , float volume = 1f){
        Sound s = Array.Find(sfx, x => x.name == name);
        if(s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
        }else{
           SfxSource.PlayOneShot(s.clip);
        }

    }
}
