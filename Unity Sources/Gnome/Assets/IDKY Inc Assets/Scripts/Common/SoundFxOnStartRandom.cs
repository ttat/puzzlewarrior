// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;

using UnityEngine;

public class SoundFxOnStartRandom : MonoBehaviour
{
    #region Fields

    public float DelaySeconds = 0f;

    public bool Loop = false;

    public float LoopDelaySeconds = 0f;

    public AudioClip[] SoundFxs;

    public int ChanceForNoSoundFxPercent = 75;

    public bool DisableSounds;

    private object myLock = new object();

    private bool playing;

    #endregion

    #region Methods

    private void OnDisable()
    {
        // Reset this flag if it gets disabled
        lock (this.myLock)
        {
            this.playing = false;
        }
    }

    private void OnEnable()
    {
        // Use OnEnable instead of Start so it will reset every time it gets re-enabled
        this.StartCoroutine(this.PlaySound(this.DelaySeconds));
    }

    private IEnumerator PlaySound(float delay)
    {
        // Choose a random number, and if it's larger or equal to ChanceForNoSoundFxPercent the play the sound,
        // otherwise don't play the sound
        int chance = Random.Range(0, 100);

        if (chance >= this.ChanceForNoSoundFxPercent)
        {
            lock (this.myLock)
            {
                this.playing = true;
            }

            yield return new WaitForSeconds(delay < 0.01f ? 0.01f : delay);

            if (!this.DisableSounds)
            {
                // Pick randomly one of the sound fx
                int index = Random.Range(0, this.SoundFxs.Length);
                SoundManager.Instance.PlaySoundFx(this.SoundFxs[index], this.transform.position);

                // Wait for the audio to have finished
                yield return new WaitForSeconds(this.SoundFxs[index].length);
            }

            lock (this.myLock)
            {
                this.playing = false;
            }
        }
    }

    private void Update()
    {
        lock (this.myLock)
        {
            if (this.Loop && !this.playing)
            {
                this.playing = true;

                this.StartCoroutine(this.PlaySound(this.LoopDelaySeconds));
            }
        }
    }

    #endregion
}