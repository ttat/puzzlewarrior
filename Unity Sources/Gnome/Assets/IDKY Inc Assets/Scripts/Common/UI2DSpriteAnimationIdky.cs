//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small script that makes it easy to create looping 2D sprite animations.
/// 
/// IDKY Change:
/// Modified to allow make looping and reversing optional.
/// Added methods to set animation frames in the editor.
/// </summary>

public class UI2DSpriteAnimationIdky : MonoBehaviour
{
    public int framerate = 20;
    public bool ignoreTimeScale = true;

    public int ReplaceFrameStartIndexInclusive = 0;
    public int ReplaceFrameEndIndexExclusive = 1;
    public int ReplaceFrameStartNumber = 0;
    public string SpriteNamePattern;
    public string SpriteSheetPath;

    public UnityEngine.Sprite[] frames;
    public bool RepeatAnimation = true;
    public bool PingPongBack = false;
    public bool DisableAfterEnded = false;

    UnityEngine.SpriteRenderer mUnitySprite;
    UI2DSprite mNguiSprite;
    int mIndex = 0;
    float mUpdate = 0f;
    private bool goingBackwards = false;

    public bool AnimationEnded { get; private set; }

    void Start()
    {
        mUnitySprite = GetComponent<UnityEngine.SpriteRenderer>();
        mNguiSprite = GetComponent<UI2DSprite>();
        if (framerate > 0) mUpdate = (ignoreTimeScale ? RealTime.time : Time.time) + 1f / framerate;
    }

    void Update()
    {
        if (framerate != 0 && frames != null && frames.Length > 0)
        {
            float time = ignoreTimeScale ? RealTime.time : Time.time;

            if (mUpdate < time)
            {
                mUpdate = time;

                if (this.RepeatAnimation && !this.PingPongBack)
                {
                    mIndex = NGUIMath.RepeatIndex(framerate > 0 ? mIndex + 1 : mIndex - 1, frames.Length);
                }
                else if (!this.RepeatAnimation && !this.PingPongBack)
                {
                    mIndex = Mathf.Min(frames.Length - 1, mIndex + 1);
                }
                else if (this.RepeatAnimation && this.PingPongBack)
                {
                    if (!this.goingBackwards)
                    {
                        mIndex++;
                        if (mIndex >= frames.Length - 1)
                        {
                            // Next time go backwards
                            this.goingBackwards = true;
                        }
                    }
                    else
                    {
                        mIndex--;
                        if (mIndex <= 0)
                        {
                            // Go forward next time
                            this.goingBackwards = false;
                        }
                    }

                    // Make sure it doesn't go out of bounds
                    mIndex = Mathf.Min(frames.Length - 1, Mathf.Max(0, mIndex));
                }
                else if (!this.RepeatAnimation && this.PingPongBack)
                {
                    if (!this.goingBackwards)
                    {
                        mIndex++;
                        if (mIndex >= frames.Length - 1)
                        {
                            // Next time go backwards
                            this.goingBackwards = true;
                        }
                    }
                    else
                    {
                        if (mIndex > 0)
                        {
                            mIndex--;
                        }
                    }

                    // Make sure it doesn't go out of bounds
                    mIndex = Mathf.Min(frames.Length - 1, Mathf.Max(0, mIndex));
                }

                mUpdate = time + Mathf.Abs(1f / framerate);

                if (mUnitySprite != null)
                {
                    mUnitySprite.sprite = frames[mIndex];
                }
                else if (mNguiSprite != null)
                {
                    mNguiSprite.nextSprite = frames[mIndex];
                }

                if (mIndex == 0 || mIndex == frames.Length - 1)
                {
                    this.AnimationEnded = true;
                    if (this.DisableAfterEnded)
                    {
                        this.gameObject.SetActive(false);
                    }
                }
                else
                {
                    this.AnimationEnded = false;
                }
            }
        }
    }

    public void RestartAnimation()
    {
        this.mIndex = 0;
    }

    [ContextMenu("SetIncrementingAnimations")]
    public void SetIncrementingAnimations()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(SpriteSheetPath);
        Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        {
            dictionary[sprite.name] = sprite;
        }

        for (int i = this.ReplaceFrameStartIndexInclusive, j = 0; i < this.ReplaceFrameEndIndexExclusive && i < this.frames.Length; i++, j++)
        {
            string key = string.Format("{0}{1}", this.SpriteNamePattern, j + this.ReplaceFrameStartNumber);
            this.frames[i] = dictionary[key];
        }
    }

    [ContextMenu("CopySpriteName")]
    public void CopySpriteName()
    {
        Sprite sprite = this.frames[this.ReplaceFrameStartIndexInclusive];

        this.SpriteNamePattern = sprite.name;
    }
}
