// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using Idky;
using UnityEngine;

public class GooglePlayGameServicesWrapper : MonoBehaviour
{
    #region Static Fields and Constants

    public const string INSTANCE_NAME = "GPGSWrapper";

    private const string ERROR_STRING = "Error";

    private const string SIGNED_OUT_STRING = "SignedOut";

    private const string SUCCESS_STRING = "Success";

    private static GooglePlayGameServicesWrapper mInstance;

    #endregion

    #region Private Fields

#if UNITY_ANDROID

    private AndroidJavaObject mCurrentActivity;

    private AndroidJavaObject mGPGSWrapper;

#endif

    #endregion
	
	#region Properties

    public string AchievementData { get; private set; }

	#endregion
	
    #region Public Methods

    public static GooglePlayGameServicesWrapper Instance()
    {
        if (mInstance != null)
            return mInstance;

        GameObject owner = new GameObject(INSTANCE_NAME);
        mInstance = owner.AddComponent<GooglePlayGameServicesWrapper>();
        DontDestroyOnLoad(mInstance);
        mInstance.AchievementData = string.Empty;
        return mInstance;
    }

    public void GPGSOnAuthResult(string result)
    {
        switch (result)
        {
            case SUCCESS_STRING:
                this.NotifySignedInSucceeded();
                break;

            case SIGNED_OUT_STRING:
                this.NotifySignedOut();
                break;

            default:
                this.NotifySignedInFailed();
                break;
        }
    }

    public void GPGSOnStateConflict(string result)
    {
        this.NotifyStateLoadConflicted(result);
    }

    public void GPGSOnStateLoaded(string result)
    {
        if (result.Contains(SUCCESS_STRING))
        {
            this.NotifyStateLoadSucceeded();
        }
        else
        {
            this.NotifyStateLoadFailed();
        }
    }

    public void GPGSOnAchievementLoaded(string result)
    {
        if (result.Contains(SUCCESS_STRING))
        {
            this.NotifyAchievementLoadSucceeded();
        }
        else
        {
            this.NotifyAchievementLoadFailed();
        }
    }

    public byte[] GetConflictedKeyLoadedData(int keyNum)
    {
        if (this.IsSignedIn())
        {
			
#if UNITY_ANDROID
			
            string str = this.mGPGSWrapper.Call<string>("getConflictData", keyNum);

            if (str != null)
            {
                byte[] data = Convert.FromBase64String(str);
                return data;
            }
			
#endif
			
            if (Debug.isDebugBuild)
            {
                // if we reach here then key was empty
                Debug.LogError("Empty data received from cloud???");
            }
        }

        return null;
    }

    public void GetAchievementData()
    {
        if (this.IsSignedIn())
        {
			string str = null;
			
#if UNITY_ANDROID
			
            str = this.mGPGSWrapper.Call<string>("getAchievementData");

            if (Debug.isDebugBuild)
            {
                Debug.Log(str);
            }
			
#endif
			
            if (str != null)
            {
                this.AchievementData = str;
            }
            else
            {
                if (Debug.isDebugBuild)
                {
                    // if we reach here then key was empty
                    Debug.LogError("Empty achievement data received from cloud");
                }
            }
        }
    }
	
    public void CloseAchievementData()
    {
        if (this.IsSignedIn())
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("closeAchievementData");
			
#endif
			
        }
    }

    public byte[] GetKeyLoadedData(int keyNum)
    {
        if (this.IsSignedIn())
        {
            string str = null;
			
#if UNITY_ANDROID
			
			str = this.mGPGSWrapper.Call<string>("getLoadedData", keyNum);
			
#endif
			
            if (str != null)
            {
                byte[] data = Convert.FromBase64String(str);
                return data;
            }

            if (Debug.isDebugBuild)
            {
                // if we reach here then key was empty
                Debug.LogError("Empty data received from cloud???");
            }
        }

        return null;
    }

    public void IncrementAchievement(string achievementId, int increment)
    {
        if (this.IsSignedIn() && !string.IsNullOrEmpty(achievementId))
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("incrementAchievement", achievementId, increment);
        
#endif
			
		}
    }

    public bool Init(string clientId)
    {
		
#if UNITY_ANDROID
		
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        this.mCurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        this.mGPGSWrapper = new AndroidJavaObject("com.idky.utilities.GooglePlayGamesServicesWrapper");
		
        if (this.mCurrentActivity != null && this.mGPGSWrapper != null)
        {
			
			
            this.mGPGSWrapper.Set("mDebugLog", true);
            this.mGPGSWrapper.SetStatic("gameObjectName", INSTANCE_NAME);
            this.mGPGSWrapper.Call<bool>("init", this.mCurrentActivity);
            return true;
        }
        else
        {
            return false;
        }
		
#else
		
		return false;
			
#endif
		
    }

    public bool IsSignedIn()
    {
		
#if UNITY_ANDROID
		
        return this.mGPGSWrapper.Call<bool>("hasAuthorised");
		
#else
		
		return false;
		
#endif
		
    }

    public void LoadAchievements()
    {
        if (this.IsSignedIn())
        {
			
#if UNITY_ANDROID
        
			this.mGPGSWrapper.Call("loadAchievements");
        
#endif
			
		}
    }

    public void LoadFromCloud(int keyNum)
    {
        if (this.IsSignedIn())
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("loadFromCloud", keyNum);
        
#endif
			
		}
    }

    public void ResolveState(int keyNum, string resolvedVersion, byte[] bytes)
    {
        if (this.IsSignedIn())
        {
            if (bytes == null || bytes.Length < 1)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogError("Empty bytes param for ResolveState");
                }

                return; // no point 
            }
			
#if UNITY_ANDROID
			
            // There was no better way i could find to pass on bytearray. so using it as string converted to base64
            string strData = Convert.ToBase64String(bytes);
            this.mGPGSWrapper.Call("resolveState", new object[] {keyNum, resolvedVersion, strData});
        
#endif
			
		}
    }

    public void SaveToCloud(int keyNum, byte[] bytes)
    {
        if (this.IsSignedIn())
        {
            if (bytes == null || bytes.Length < 1)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogError("Empty bytes param for saveToCloud");
                }

                return; // no point 
            }
			
#if UNITY_ANDROID
			
            // There was no better way i could find to pass on bytearray. so using it as string converted to base64
            string strData = Convert.ToBase64String(bytes);
            this.mGPGSWrapper.Call("saveToCloud", keyNum, strData);
			
#endif
			
		}
    }

    public void SetGameObjectName(string gameObjectName)
    {
		
#if UNITY_ANDROID
		
        this.mGPGSWrapper.SetStatic("gameObjectName", gameObjectName);
		
#endif
		
    }

    public void ShowAchievements()
    {
        if (this.IsSignedIn())
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("showAchievements");
			
#endif
			
        }
    }

    public void ShowAllLeaderBoards()
    {
        if (this.IsSignedIn())
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("showAllLeaderBoards");
			
#endif
			
        }
    }

    public void ShowLeaderBoards(string leaderBoardId)
    {
        if (this.IsSignedIn() && !string.IsNullOrEmpty(leaderBoardId))
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("showLeaderBoards", leaderBoardId);
			
#endif
			
        }
    }

    public bool SignIn()
    {
		
#if UNITY_ANDROID
        
		return this.mGPGSWrapper.Call<bool>("signIn");
    
#else
		
		return false;
		
#endif
		
	}

    public void SignOut()
    {
		
#if UNITY_ANDROID
        
		this.mGPGSWrapper.Call("signOut");

#endif
		
	}

    public void SubmitScore(string leaderBoardId, long score)
    {
        if (this.IsSignedIn() && !string.IsNullOrEmpty(leaderBoardId))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Submitting scrore for " + leaderBoardId + ": " + score);
            }
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("submitScore", leaderBoardId, score);
        
#endif
			
		}
    }

    public void UnlockAchievement(string achievementId)
    {
        if (this.IsSignedIn() && !string.IsNullOrEmpty(achievementId))
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("unlockAchievement", achievementId);
        
#endif
			
		}
    }

    public void RevealAchievement(string achievementId)
    {
        if (this.IsSignedIn() && !string.IsNullOrEmpty(achievementId))
        {
			
#if UNITY_ANDROID
			
            this.mGPGSWrapper.Call("revealAchievement", achievementId);
        
#endif
			
		}
    }

    #endregion

    #region Private and Protected Methods

    private void NotifySignedInFailed()
    {
        if (this.SignedInFailed != null)
        {
            this.SignedInFailed(this, new EventArgs());
        }
    }

    private void NotifySignedInSucceeded()
    {
        if (this.SignedInSucceeded != null)
        {
            this.SignedInSucceeded(this, new EventArgs());
        }
    }

    private void NotifySignedOut()
    {
        if (this.SignedOut != null)
        {
            this.SignedOut(this, new EventArgs());
        }
    }

    private void NotifyStateLoadConflicted(string result)
    {
        if (this.StateLoadConflicted != null)
        {
            this.StateLoadConflicted(this, new ResolveConflictEventArgs(result));
        }
    }

    private void NotifyStateLoadFailed()
    {
        if (this.StateLoadFailed != null)
        {
            this.StateLoadFailed(this, new EventArgs());
        }
    }

    private void NotifyStateLoadSucceeded()
    {
        if (this.StateLoadSucceeded != null)
        {
            this.StateLoadSucceeded(this, new EventArgs());
        }
    }

    private void NotifyAchievementLoadFailed()
    {
        if (this.AchievementLoadFailed != null)
        {
            this.AchievementLoadFailed(this, new EventArgs());
        }
    }

    private void NotifyAchievementLoadSucceeded()
    {
        if (this.AchievementLoadSucceeded != null)
        {
            this.AchievementLoadSucceeded(this, new EventArgs());
        }
    }

    #endregion

    public event EventHandler<EventArgs> SignedInSucceeded;

    public event EventHandler<EventArgs> SignedInFailed;

    public event EventHandler<EventArgs> SignedOut;

    public event EventHandler<EventArgs> StateLoadSucceeded;

    public event EventHandler<EventArgs> StateLoadFailed;

    public event EventHandler<EventArgs> AchievementLoadSucceeded;

    public event EventHandler<EventArgs> AchievementLoadFailed;

    public event EventHandler<ResolveConflictEventArgs> StateLoadConflicted;
}