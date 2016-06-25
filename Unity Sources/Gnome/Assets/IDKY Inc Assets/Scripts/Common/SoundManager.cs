// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;

using Idky;

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Fields

    public AudioSource BackgroundMusicSource;

    public AudioSource ContinuousSoundSource;

    public string MusicOptionKey = "MusicOptionKey";

    public bool Paused;

    public bool PlayMusic;

    public bool PlaySounds;

    public string SoundOptionKey = "SoundOptionKey";

    #endregion

    #region Public Properties

    public static SoundManager Instance { get; private set; }

    public float CurrentPitch
    {
        get
        {
            if (this.BackgroundMusicSource != null)
            {
                return this.BackgroundMusicSource.pitch;
            }
            else
            {
                return 1;
            }
        }
    }

    #endregion

    #region Public Methods and Operators

    public void ClearBackgroundMusic()
    {
        this.BackgroundMusicSource.Stop();
        this.BackgroundMusicSource.clip = null;
    }

    public void ClearContinuousSoundFx()
    {
        this.ContinuousSoundSource.Stop();
        this.ContinuousSoundSource.clip = null;
    }

    public void Pause()
    {
        this.BackgroundMusicSource.Pause();
        this.ContinuousSoundSource.Pause();

        this.Paused = true;
    }

    public void PlayBackgroundMusic(AudioClip clip, bool loop)
    {
        // Already playing it, so just let it keep playing
        if (this.BackgroundMusicSource.clip == clip)
        {
            return;
        }

        this.BackgroundMusicSource.clip = clip;
        this.BackgroundMusicSource.loop = loop;

        if (this.PlayMusic && this.BackgroundMusicSource.clip != null)
        {
            this.BackgroundMusicSource.Play();
        }
    }

    public void PlayContinuousSoundFx(AudioClip clip)
    {
        this.ContinuousSoundSource.clip = clip;

        if (this.PlaySounds && this.ContinuousSoundSource.clip != null)
        {
            this.ContinuousSoundSource.Play();
        }
    }

    public void PlaySoundFx(AudioClip clip, Vector3 position)
    {
        if (this.PlaySounds)
        {
            if (Time.timeScale == 0)
            {
                // Sounds don't play if timeScale = 0, so it needs to create and destroy a game object to play
                // the sound
                this.StartCoroutine("PlaySoundFxWhilePaused", new object[] { clip, position });
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, position);
            }
        }
    }

    public void RecheckSounds()
    {
        this.CheckOptions();

        if (this.PlayMusic && this.BackgroundMusicSource.clip != null && !this.BackgroundMusicSource.isPlaying && !this.Paused)
        {
            // Should be playing, but isn't
            this.BackgroundMusicSource.Play();
        }
        else if ((!this.PlayMusic || this.Paused) && this.BackgroundMusicSource.isPlaying)
        {
            // Playing when it shouldn't be
            this.BackgroundMusicSource.Stop();
        }

        if (this.PlaySounds && this.ContinuousSoundSource.clip != null && !this.ContinuousSoundSource.isPlaying && !this.Paused)
        {
            // Should be playing, but isn't
            this.ContinuousSoundSource.Play();
        }
        else if ((!this.PlayMusic || this.Paused) && this.ContinuousSoundSource.isPlaying)
        {
            // Playing when it shouldn't be
            this.ContinuousSoundSource.Stop();
        }

        if (!this.PlaySounds)
        {
            foreach (AudioSource audioClip in FindObjectsOfType<AudioSource>())
            {
                audioClip.Stop();
            }
        }
    }

    public void SetBackgroundPitch(float pitch)
    {
        if (this.BackgroundMusicSource != null)
        {
            this.BackgroundMusicSource.pitch = pitch;
        }
    }

    public void Unpause()
    {
        this.Paused = false;

        this.CheckOptions();

        if (this.PlayMusic && this.BackgroundMusicSource.clip != null)
        {
            this.BackgroundMusicSource.Play();
        }

        if (this.PlaySounds && this.ContinuousSoundSource.clip != null)
        {
            this.ContinuousSoundSource.Play();
        }
    }

    #endregion

    #region Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        Instance = this;
        this.Paused = false;

        // Make sure music and sound fx is on by default if it's never been initialized
        if (!PlayerPrefsFast.HasKey(this.MusicOptionKey))
        {
            PlayerPrefsFast.SetBool(this.MusicOptionKey, true);
            PlayerPrefsFast.Flush();
        }

        if (!PlayerPrefsFast.HasKey(this.SoundOptionKey))
        {
            PlayerPrefsFast.SetBool(this.SoundOptionKey, true);
            PlayerPrefsFast.Flush();
        }

        this.CheckOptions();
        this.BackgroundMusicSource = this.gameObject.AddComponent<AudioSource>();
        this.BackgroundMusicSource.loop = true;
        this.ContinuousSoundSource = this.gameObject.AddComponent<AudioSource>();
        this.ContinuousSoundSource.loop = true;

        DontDestroyOnLoad(this.gameObject);
    }

    private void CheckOptions()
    {
        // Check if music and sound fx can be played
        this.PlayMusic = PlayerPrefsFast.GetBool(this.MusicOptionKey);
        this.PlaySounds = PlayerPrefsFast.GetBool(this.SoundOptionKey);
    }

    private IEnumerator PlaySoundFxWhilePaused(object param)
    {
        object[] parameters = (object[])param;
        AudioClip clip = (AudioClip)parameters[0];
        Vector3 position = (Vector3)parameters[1];

        // Create a temporary game object and play the sound
        GameObject temp = new GameObject();
        temp.transform.position = position;
        temp.AddComponent<AudioSource>();
        temp.audio.clip = clip;
        temp.audio.Play();

        // Wait for it to finish playing
        yield return new WaitForSeconds(clip.length);

        // Then destroy the game object
        Destroy(temp);
    }

    #endregion
}