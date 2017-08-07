using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = new AudioManager();
    public AudioSource backgroundAudioSource;
    public List<AudioClip> backgroundMusic;
     #region Instance and things

    static AudioManager()
    {
    }

    private AudioManager()
    {
    }

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        ShuffleSong();
        backgroundAudioSource.volume = 0;
        backgroundAudioSource.Play();
    }

    void Update()
    {
        GrowAudio();

        if (!backgroundAudioSource.isPlaying)
        {
            ShuffleSong();
        }
    }





    public void PlayRandomVolWithSrc(AudioClip audioToBePlayed, AudioSource audioSrcToBeUsed)
    {
        float randomVol = Random.Range(0.1f, 1f);
        audioSrcToBeUsed.PlayOneShot(audioToBePlayed, randomVol);
    }

    public void ShuffleSong()
    {
        int randomSongID = Random.Range(0, backgroundMusic.Count);
        backgroundAudioSource.Stop();
        Debug.Log(randomSongID);
        backgroundAudioSource.PlayOneShot(backgroundMusic[randomSongID]);
    }

    void GrowAudio()
    {
        if (backgroundAudioSource.volume != 1)
        {
            backgroundAudioSource.volume = Mathf.Lerp(backgroundAudioSource.volume, 1, Time.deltaTime * 0.01f);
        }
    }

  }

   
